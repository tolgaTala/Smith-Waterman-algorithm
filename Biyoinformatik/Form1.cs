using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;

namespace Biyoinformatik
{
    public partial class Form1 : Form
    {
        static string[] seq1 = System.IO.File.ReadAllLines(@"C:\Users\tolga\source\repos\Biyoinformatik\seq1.txt");
        static string[] seq2 = System.IO.File.ReadAllLines(@"C:\Users\tolga\source\repos\Biyoinformatik\seq2.txt");

        int seq1Count = Convert.ToInt32(seq1[0]) + 1;
        string seq1String = seq1[1];

        int seq2Count = Convert.ToInt32(seq2[0]) + 1;
        string seq2String = seq2[1];

        int match = 1;
        int misMmatch = -1;
        int gap = -2;

        Stopwatch watch = new Stopwatch();

        public Form1()
        {
            InitializeComponent();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Text files | *.txt";
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string seq1Path = dialog.FileName;
                seq1 = System.IO.File.ReadAllLines(seq1Path);
                seq1Count = Convert.ToInt32(seq1[0]) + 1;
                seq1String = seq1[1];
                label8.Text = seq1Path;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Text files | *.txt";
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string seq2Path = dialog.FileName;
                seq2 = System.IO.File.ReadAllLines(seq2Path);
                seq2Count = Convert.ToInt32(seq2[0]) + 1;
                seq2String = seq2[1];
                label9.Text = seq2Path;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            watch.Reset();
            watch.Start();

            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            dataGridView1.Refresh();

            dataGridView1.Columns.Add("asd", " ");
            dataGridView1.Rows.Add();
            dataGridView1.Rows[0].Cells[0].Value = "0".ToString();

            label1.Text = seq1String;
            label2.Text = seq2String;

            for (int i = 0; i < seq1Count - 1; i++)
            {
                dataGridView1.Columns.Add("sembol", seq1String[i].ToString());
            }
            for (int j = 0; j < seq2Count - 1; j++)
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[j + 1].HeaderCell.Value = seq2String[j].ToString();
            }

            char[] seq2Array = seq2String.ToCharArray();
            char[] seq1Array = seq1String.ToCharArray();

            match = Convert.ToInt32(textBox1.Text);
            misMmatch = Convert.ToInt32(textBox2.Text);
            gap = Convert.ToInt32(textBox3.Text);

            //Convert.ToInt32(dataGridView1.Rows[i].Cells[j].Value

            dataGridView1.Rows[0].Cells[0].Value = 0;

            int seq2sayac = 0;
            int seq1sayac = 0;

            //Tablodaki uzunlarına göre dönüyor.
            for (int i = 0; i < seq2Count; i++)
            {
                for (int j = 0; j < seq1Count; j++)
                {
                    //sol üst köşede hiç bir işlem yapmadan devam etmesini için
                    if (j == 0 && i == 0)
                    {
                        continue;
                    }

                    int scoreDiag = 0;
                    int scoreleft = 0;
                    int scoreUp = 0;
                    //ilk değer için eşleşme var mı yok mu diye bakıyor.
                    if (seq2Array[seq2sayac] == seq1Array[seq1sayac])
                    {
                        //ikisinden biri 0 olursa demekki scorediag işleme girmemeli 
                        if (i == 0 || j == 0)
                        {
                            scoreDiag = -1000;
                        }
                        else
                        {
                            scoreDiag = Convert.ToInt32(dataGridView1.Rows[i - 1].Cells[j - 1].Value) + match;
                        }
                    }
                    else
                    {
                        if (i == 0 || j == 0)
                        {
                            scoreDiag = -1000;
                        }
                        else
                        {
                            scoreDiag = Convert.ToInt32(dataGridView1.Rows[i - 1].Cells[j - 1].Value) + misMmatch;
                        }

                    }

                    if (i != 0)
                    {
                        scoreleft = Convert.ToInt32(dataGridView1.Rows[i - 1].Cells[j].Value) + gap;
                    }
                    else
                    {
                        scoreleft = -1000;
                    }
                    if (j != 0)
                    {
                        scoreUp = Convert.ToInt32(dataGridView1.Rows[i].Cells[j - 1].Value) + gap;
                    }
                    else
                    {
                        scoreUp = -1000;
                    }

                    int maxScore = Math.Max(Math.Max(scoreDiag, scoreleft), scoreUp);
                    //Üç hesaplama sonucunda maksimum değeri alıp tabloya ekliyoruz.
                    dataGridView1.Rows[i].Cells[j].Value = maxScore;

                    //Sekansı uygun gezmek için eşitliyoruz.
                    seq1sayac = j;
                    //tablo uzunluğumuz sekans uzunluğundan 1 fazla olduğundan out of range yememek için.
                    if (seq1sayac == seq1Array.Length)
                    {
                        seq1sayac--;
                    }
                }
                //aynı şekilde ikinci sekans içinde aynısını uyguluyoruz.
                seq2sayac = i;

                if (seq2sayac == seq2Array.Length)
                {
                    seq2sayac--;
                }
            }

            //Geri takip adımları
            //Hizalama ve maksimum skor bulma

            string HizaliSeq1 = string.Empty;
            string HizaliSeq2 = string.Empty;

            //Sağ alt köşe değerinden başlamak için.
            int m = seq2Count - 1;
            int n = seq1Count - 1;

            var index1 = dataGridView1.Rows[m].Cells[n];
            index1.Style.BackColor = Color.Red;

            int aligmentMaxScore = 0;

            //Sol üste gelene kadar çalışıcak
            while (m > 0 || n > 0)
            {
                int scroeDiag = 0;

                //iki sekanstan birinin değeri biterse karşılarına boşluk atmak için
                if (m == 0 && n > 0)
                {
                    HizaliSeq1 = seq1Array[n - 1] + HizaliSeq1;
                    HizaliSeq2 = "-" + HizaliSeq2;
                    dataGridView1.Rows[m].Cells[n - 1].Style.BackColor = Color.Red;
                    n = n - 1;
                    aligmentMaxScore = aligmentMaxScore + gap;
                }
                else if (n == 0 && m > 0)
                {
                    HizaliSeq1 = "-" + HizaliSeq1;
                    HizaliSeq2 = seq2Array[m - 1] + HizaliSeq2;
                    dataGridView1.Rows[m - 1].Cells[n].Style.BackColor = Color.Red;
                    m = m - 1;
                    aligmentMaxScore = aligmentMaxScore + gap;
                }
                //sekanslar doluysa
                else
                {

                    if (seq2Array[m - 1] == seq1Array[n - 1])
                        scroeDiag = match;
                    else
                        scroeDiag = misMmatch;

                    if (m > 0 && n > 0 && Convert.ToInt32(dataGridView1.Rows[m].Cells[n].Value) ==
                        Convert.ToInt32(dataGridView1.Rows[m - 1].Cells[n - 1].Value) + scroeDiag)
                    {
                        dataGridView1.Rows[m - 1].Cells[n - 1].Style.BackColor = Color.Red;
                        HizaliSeq1 = seq1Array[n - 1] + HizaliSeq1;
                        HizaliSeq2 = seq2Array[m - 1] + HizaliSeq2;
                        m = m - 1;
                        n = n - 1;
                        aligmentMaxScore = aligmentMaxScore + scroeDiag;
                    }

                    else if (n > 0 && Convert.ToInt32(dataGridView1.Rows[m].Cells[n].Value) ==
                        Convert.ToInt32(dataGridView1.Rows[m].Cells[n - 1].Value) + gap)
                    {
                        dataGridView1.Rows[m].Cells[n - 1].Style.BackColor = Color.Red;
                        HizaliSeq1 = seq1Array[n - 1] + HizaliSeq1;
                        HizaliSeq2 = "-" + HizaliSeq2;
                        n = n - 1;
                        aligmentMaxScore = aligmentMaxScore + gap;
                    }

                    else if (m > 0 && Convert.ToInt32(dataGridView1.Rows[m].Cells[n].Value) ==
                        Convert.ToInt32(dataGridView1.Rows[m - 1].Cells[n].Value) + gap)
                    {
                        dataGridView1.Rows[m-1].Cells[n].Style.BackColor = Color.Red;
                        HizaliSeq1 = "-" + HizaliSeq1;
                        HizaliSeq2 = seq2Array[m - 1] + HizaliSeq2;
                        m = m - 1;
                        aligmentMaxScore = aligmentMaxScore + gap;
                    }
                }
            }
            label1.Text = HizaliSeq1;
            label2.Text = HizaliSeq2;
            label4.Text = "Maksimum Skor = " + aligmentMaxScore.ToString();

            //algoritmanın çalışma süresini hesaplama
            watch.Stop();
            label3.Text = (watch.ElapsedMilliseconds / 1000.0).ToString() + " Milisaniyede tamamlandı.";            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = match.ToString();
            textBox2.Text = misMmatch.ToString();
            textBox3.Text = gap.ToString();           
        }

        
    }
}
