using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace teh_zrenie2
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }
        Thread IA, Loading, Etalon, Ekzamen;
        string pathfolder;
        List<Vectors> VectorsImage,
                      massiveEtalons;
        string ending;
        private List<Vectors> MyCopyTo(List<Vectors> vector)
        {
            List<Vectors> temp = new List<Vectors>();
            foreach (var item in vector)
            {
                temp.Add(item);
            }
            return temp;
        }
        private void SaveClassEtalon(int[] counter, List<int[]> list)
        {
            SaveFileDialog SF = new SaveFileDialog
            {
                Title = "Выбери куда сохранить отчёт",
                FileName = "errCount",
                Filter = "text |*.txt",
                DefaultExt = "txt"
            };
            if (SF.ShowDialog() == DialogResult.OK)
            {
                if (SF.FileName != "")
                {
                    using (StreamWriter file = new StreamWriter(SF.FileName, false))
                    {
                        file.WriteLine("Ошибки по классам: ");
                        richTextBox1.Text += "Ошибки по классам: \r\n";
                        for (int i = 1; i < counter.Length; i++)
                        {
                            file.WriteLine("Класс " + i + "  " + counter[i]);
                            richTextBox1.Text += "Класс " + i + "  " + counter[i] + "\r\n";
                        }
                        richTextBox1.Text += "Всего эталонов: " + list.Count + "\r\n";
                        file.WriteLine("Всего эталонов: " + list.Count);
                        foreach (var item in list)
                        {
                            if (item[0] != 0)
                            {
                                file.WriteLine("Класс " + item[1] + "  Количество распознаных  " + item[0]);
                                richTextBox1.Text += "Класс " + item[1] + "  Количество распознаных  " + item[0] + "\r\n";
                            }
                            else
                            {
                                file.WriteLine("Класс " + item[1] + " Не использовался ");
                                richTextBox1.Text += "Класс " + item[1] + " Не использовался \r\n";
                            }
                        }
                    }
                }
            }
            else MessageBox.Show("Пустое Имя Файла, НЕ СОХРАНЕНО");
        }
        private string PathFolder()
        {
            FolderBrowserDialog FBD = new FolderBrowserDialog
            {
                RootFolder = Environment.SpecialFolder.MyComputer,
                Description = "ВЫБЕРИ ПАПКУ С ИЗОБРАЖЕНИЯМИ"
            };
            if (FBD.ShowDialog() == DialogResult.OK)
            {
                pathfolder = FBD.SelectedPath;
                return FBD.SelectedPath;
            }
            return String.Empty;
        }
        private void scan_button_Click(object sender, EventArgs e)
        {
            string path = PathFolder();
            if (path != String.Empty)
            {
                int count = Directory.GetFiles(path, "*.png", SearchOption.AllDirectories).Length;
                ToBlock(true);
                var MB = MessageBox.Show("Найдено " + count + " изображений. Сканировать?", "Сообщение", MessageBoxButtons.YesNo);
                if (MB == DialogResult.Yes)
                {
                    IA = new Thread(new ParameterizedThreadStart(ImageAnaliz));
                    IA.Start(path);
                    ending = "scan";
                    timer1.Enabled = true;
                }
            }
        }
        private void ToBlock(bool b)
        {
            if (b)
            {
                scan_button.Enabled = false;
                load_button.Enabled = false;
                etalon_button.Enabled = false;
                ekzamen_button.Enabled = false;
            }
            else
            {
                scan_button.Enabled = true;
                load_button.Enabled = true;
                etalon_button.Enabled = true;
                ekzamen_button.Enabled = true;
            }
            richTextBox1.SelectionStart = richTextBox1.TextLength;
        }
        private void ImageAnaliz(object path1)
        {
            string path = path1.ToString();
            if (path != String.Empty)
            {
                string[] pathFiles = Directory.GetFiles(path, "*.png", SearchOption.AllDirectories);
                AttributeSearch attributeSearch = new AttributeSearch();
                Bar_prog.Invoke(new Action(() => { Bar_prog.Maximum = pathFiles.Length; }));
                using (StreamWriter SW = new StreamWriter(path + @"\FilesInfo.txt", false))
                {
                    for (int i = 0; i < pathFiles.Length; i++)
                    {
                        Bitmap image = new Bitmap(pathFiles[i]);
                        Vectors vector = new Vectors();
                        vector.ListPointsImage = attributeSearch.ListPointsImage(image);
                        vector.CountPointsImage = attributeSearch.CounterPoints(image);
                        vector.CounterPointsContour = attributeSearch.CounterPointsContour(vector.ListPointsImage);
                        vector.CounterWay = attributeSearch.CounterWay(vector.ListPointsImage);
                        vector.CounterLength = attributeSearch.CounterLength(vector.CounterWay);
                        vector.CounterAngle = attributeSearch.CounterAngle(vector.ListPointsImage);
                        string temp = pathFiles[i].Remove(0, path.Length + 1);
                        if (temp.StartsWith("10"))
                            vector.ClassImage = 10;
                        else if (temp.StartsWith("2"))
                            vector.ClassImage = 2;
                        else if (temp.StartsWith("3"))
                            vector.ClassImage = 3;
                        else if (temp.StartsWith("4"))
                            vector.ClassImage = 4;
                        else if (temp.StartsWith("5"))
                            vector.ClassImage = 5;
                        else if (temp.StartsWith("6"))
                            vector.ClassImage = 6;
                        else if (temp.StartsWith("7"))
                            vector.ClassImage = 7;
                        else if (temp.StartsWith("8"))
                            vector.ClassImage = 8;
                        else if (temp.StartsWith("9"))
                            vector.ClassImage = 9;
                        else if (temp.StartsWith("1"))
                            vector.ClassImage = 1;
                        SW.WriteLine(
                            + vector.CountPointsImage + ";"
                            + vector.CounterPointsContour + ";"
                            + vector.CounterWay[0] + ";"
                            + vector.CounterWay[1] + ";"
                            + vector.CounterWay[2] + ";"
                            + vector.CounterWay[3] + ";"
                            + vector.CounterWay[4] + ";"
                            + vector.CounterWay[5] + ";"
                            + vector.CounterWay[6] + ";"
                            + vector.CounterWay[7] + ";"
                            + vector.CounterLength + ";"
                            + vector.CounterAngle[0] + ";"
                            + vector.CounterAngle[1] + ";"
                            + vector.CounterAngle[2] + ";"
                            + vector.ClassImage
                            );
                        Bar_prog.Invoke(new Action(() => { Bar_prog.Value = i+1; }));
                    }
                }
            }
        }
        private void load_button_Click(object sender, EventArgs e)
        {
            OpenFileDialog OF = new OpenFileDialog
            {
                InitialDirectory = pathfolder,
                FileName = "FileInfo.txt",
                Filter = "FileInfo|*.txt|All files|*.*"
            };
            
            if (OF.ShowDialog() == DialogResult.OK)
            {
                ending = "load";
                Loading = new Thread(new ParameterizedThreadStart(loader));
                Loading.Start(OF.FileName);
                ToBlock(true);
                timer1.Enabled = true;
            }
        }
        private void loader(object fileName)
        {
            try
            {
                int count = File.ReadAllLines(fileName.ToString()).Length;
                Bar_prog.Invoke(new Action(() => { Bar_prog.Maximum = count; }));
                using (StreamReader SR = new StreamReader(fileName.ToString()))
                {
                    VectorsImage = new List<Vectors>();
                    while (!SR.EndOfStream)
                    {
                        string[] str = SR.ReadLine().Split(';');
                        Vectors vector = new Vectors();
                        vector.CounterWay = new int[8];
                        vector.CounterAngle = new int[3];
                        vector.CountPointsImage = int.Parse(str[0]);
                        vector.CounterPointsContour = int.Parse(str[1]);
                        vector.CounterWay[0] = int.Parse(str[2]);
                        vector.CounterWay[1] = int.Parse(str[3]);
                        vector.CounterWay[2] = int.Parse(str[4]);
                        vector.CounterWay[3] = int.Parse(str[5]);
                        vector.CounterWay[4] = int.Parse(str[6]);
                        vector.CounterWay[5] = int.Parse(str[7]);
                        vector.CounterWay[6] = int.Parse(str[8]);
                        vector.CounterWay[7] = int.Parse(str[9]);
                        vector.CounterLength = int.Parse(str[10]);
                        vector.CounterAngle[0] = int.Parse(str[11]);
                        vector.CounterAngle[1] = int.Parse(str[12]);
                        vector.CounterAngle[2] = int.Parse(str[13]);
                        vector.ClassImage = int.Parse(str[14]);
                        VectorsImage.Add(vector);
                        Bar_prog.Invoke(new Action(() => { Bar_prog.Value++; }));
                    }
                }
            }
            catch( Exception ex)
            { MessageBox.Show("Не удалось загрузить!\r\n"+ex); }
        }
        private void etalon_button_Click(object sender, EventArgs e)
        {
            massiveEtalons = new List<Vectors>();
            ending = "etalon";
            Etalon = new Thread(new ThreadStart(searchEtalon));
            Etalon.Start();
            ToBlock(true);
            timer1.Enabled = true;
        }
        private void searchEtalon()
        {
            Random rn = new Random();
            List<Vectors> vct = new List<Vectors>();
            vct = MyCopyTo(VectorsImage);
            Bar_prog.Invoke(new Action(() => { Bar_prog.Maximum = vct.Count; }));
            while (true)
            {
                int etalon = rn.Next(vct.Count);
                int countEtalon = 0;

                while (true)
                {
                    List<int> d = new List<int>();
                    searchD(vct, vct, etalon - countEtalon, d);
                    int index = d.IndexOf((from x in d where x > 0 select x).Min());
                    if (vct[index].ClassImage == vct[etalon - countEtalon].ClassImage)//etalonVector
                    {
                        vct.RemoveAt(index);
                        if (index < etalon - countEtalon)
                            countEtalon++;
                    }
                    else
                    {
                        massiveEtalons.Add(vct[etalon - countEtalon]);
                        vct.RemoveAt(etalon - countEtalon);
                        countEtalon = 0;
                        break;
                    }
                    Bar_prog.Invoke(new Action(() => { Bar_prog.Value = Bar_prog.Maximum - vct.Count; }));
                    if (vct.Count == 1)
                    {
                        massiveEtalons.Add(vct[0]);
                        break;
                    }
                }
                if (vct.Count == 1)
                    break;
            }
            Bar_prog.Invoke(new Action(() => { Bar_prog.Value = Bar_prog.Maximum; }));
        }
        private void searchD(List<Vectors> vct, List<Vectors> vct2, int etalon, List<int> d)
        {
            for (int i = 0; i < vct.Count; i++)
            {
                d.Add((int)Math.Sqrt(
                                   +Math.Pow((vct[etalon].CountPointsImage - vct2[i].CountPointsImage), 2)
                                   + Math.Pow((vct[etalon].CounterPointsContour - vct2[i].CounterPointsContour), 2)
                                   + Math.Pow((vct[etalon].CounterWay[0] - vct2[i].CounterWay[0]), 2)
                                   + Math.Pow((vct[etalon].CounterWay[1] - vct2[i].CounterWay[1]), 2)
                                   + Math.Pow((vct[etalon].CounterWay[2] - vct2[i].CounterWay[2]), 2)
                                   + Math.Pow((vct[etalon].CounterWay[3] - vct2[i].CounterWay[3]), 2)
                                   + Math.Pow((vct[etalon].CounterWay[4] - vct2[i].CounterWay[4]), 2)
                                   + Math.Pow((vct[etalon].CounterWay[5] - vct2[i].CounterWay[5]), 2)
                                   + Math.Pow((vct[etalon].CounterWay[6] - vct2[i].CounterWay[6]), 2)
                                   + Math.Pow((vct[etalon].CounterWay[7] - vct2[i].CounterWay[7]), 2)
                                   + Math.Pow((vct[etalon].CounterLength - vct2[i].CounterLength), 2)
                                   + Math.Pow((vct[etalon].CounterAngle[0] - vct2[i].CounterAngle[0]), 2)
                                   + Math.Pow((vct[etalon].CounterAngle[1] - vct2[i].CounterAngle[1]), 2)
                                   + Math.Pow((vct[etalon].CounterAngle[2] - vct2[i].CounterAngle[2]), 2)
                                   ));
            }
        }
        private void searchD2(List<Vectors> vct, List<Vectors> vct2, int etalon, List<int> d)
        {
            for (int i = 0; i < vct2.Count; i++)
            {
                d.Add((int)Math.Sqrt(
                                   +Math.Pow((vct[etalon].CountPointsImage - vct2[i].CountPointsImage), 2)
                                   + Math.Pow((vct[etalon].CounterPointsContour - vct2[i].CounterPointsContour), 2)
                                   + Math.Pow((vct[etalon].CounterWay[0] - vct2[i].CounterWay[0]), 2)
                                   + Math.Pow((vct[etalon].CounterWay[1] - vct2[i].CounterWay[1]), 2)
                                   + Math.Pow((vct[etalon].CounterWay[2] - vct2[i].CounterWay[2]), 2)
                                   + Math.Pow((vct[etalon].CounterWay[3] - vct2[i].CounterWay[3]), 2)
                                   + Math.Pow((vct[etalon].CounterWay[4] - vct2[i].CounterWay[4]), 2)
                                   + Math.Pow((vct[etalon].CounterWay[5] - vct2[i].CounterWay[5]), 2)
                                   + Math.Pow((vct[etalon].CounterWay[6] - vct2[i].CounterWay[6]), 2)
                                   + Math.Pow((vct[etalon].CounterWay[7] - vct2[i].CounterWay[7]), 2)
                                   + Math.Pow((vct[etalon].CounterLength - vct2[i].CounterLength), 2)
                                   + Math.Pow((vct[etalon].CounterAngle[0] - vct2[i].CounterAngle[0]), 2)
                                   + Math.Pow((vct[etalon].CounterAngle[1] - vct2[i].CounterAngle[1]), 2)
                                   + Math.Pow((vct[etalon].CounterAngle[2] - vct2[i].CounterAngle[2]), 2)
                                   ));
            }
        }
        private void ekzamen_button_Click(object sender, EventArgs e)
        {
            ending = "ekzamen";
            Ekzamen = new Thread(new ThreadStart(ExzamenSys));
            Ekzamen.Start();
            ToBlock(true);
            timer1.Enabled = true;
        }
        private void ExzamenSys()
        {
            int[] counterError = new int[11];//for err
            List<Vectors> vct = new List<Vectors>();
            vct = MyCopyTo(VectorsImage);//изображения там
            List<Vectors> newMassiveEtalons = new List<Vectors>();
            newMassiveEtalons = MyCopyTo(massiveEtalons);//массивы здесь
            Random rn = new Random();
            Bar_prog.Invoke(new Action(() => { Bar_prog.Maximum = vct.Count; }));
            while (true)
            {
                int indexRandomImage = rn.Next(vct.Count);
                while (true)
                {
                    List<int> d = new List<int>();
                    searchD2(vct, newMassiveEtalons, indexRandomImage, d);
                    int ind = d.IndexOf(d.Min());
                    if (newMassiveEtalons[ind].ClassImage == vct[indexRandomImage].ClassImage)//etalonVector
                    {
                        vct.RemoveAt(indexRandomImage);
                        massiveEtalons[massiveEtalons.IndexOf(newMassiveEtalons[ind])].recog++;//считаем верно распознаные 
                        newMassiveEtalons = MyCopyTo(massiveEtalons);
                        break;
                    }
                    else
                    {
                        counterError[newMassiveEtalons[ind].ClassImage]++;
                        newMassiveEtalons.RemoveAt(ind);
                        Bar_prog.Invoke(new Action(() => { Bar_prog.Value = Bar_prog.Maximum - vct.Count; }));
                        if (vct.Count == 0)
                            break;
                    }
                }
                if (vct.Count == 0)
                    break;
            }

            List<int[]> countRecog = new List<int[]>();
            foreach (var item in massiveEtalons)
            {
                countRecog.Add(new int[] { item.recog, item.ClassImage }) ;
            }
            this.Invoke(new Action(() => { SaveClassEtalon(counterError, countRecog); }));
            Bar_prog.Invoke(new Action(() => { Bar_prog.Value = Bar_prog.Maximum; }));
        }
        private void timer1_Tick_1(object sender, EventArgs e)//3c
        {
            if (Bar_prog.Value == Bar_prog.Maximum)
            {
                if (ending.StartsWith("scan"))
                {
                    IA.Abort();
                    timer1.Enabled = false;
                    ToBlock(false);
                    richTextBox1.Text += "Вектора созданы и сохранены\r\n";
                }
                else if (ending.StartsWith("load"))
                {
                    Loading.Abort();
                    timer1.Enabled = false;
                    ToBlock(false);
                    richTextBox1.Text += "Вектора загружены\r\n";
                    etalon_button.Enabled = true;
                }
                else if(ending.StartsWith("etalon"))
                {
                    Etalon.Abort();
                    timer1.Enabled = false;
                    ToBlock(false);
                    richTextBox1.Text += "Эталоны созданы\r\n";
                    int[] te = new int[11];
                    foreach (var item in massiveEtalons)
                    {
                        te[item.ClassImage]++;
                    }
                    richTextBox1.Text += "Количество эталонов:\r\n";
                    for (int i = 1; i < te.Length; i++)
                    {
                        richTextBox1.Text += "Класс " + i + " = " + te[i] + "\r\n";
                    }
                }
                else if (ending.StartsWith("ekzamen"))
                {
                    Ekzamen.Abort();
                    timer1.Enabled = false;
                    ToBlock(false);
                    richTextBox1.Text += "Экзамен завершен\r\n";
                }
                Bar_prog.Value = 0;
            }
        }
    }
}
