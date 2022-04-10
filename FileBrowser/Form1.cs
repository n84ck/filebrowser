using FileBrowser.Models;
using FileBrowser.Repository;
using Newtonsoft.Json;
using System.Net;

namespace FileBrowser
{
    public partial class Form1 : Form
    {
        private readonly IDataRepository dataRepository;

        public Form1(IDataRepository dataRepository)
        {
            InitializeComponent();
            this.dataRepository = dataRepository;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var res = await dataRepository.GetFileAsync(textBox1.Text);
            if (res.IsSuccessful && res.Result != null)
            {
                var sampleArray = res.Result.Content.Take(100).ToArray();
                textBox2.Text = System.Text.Encoding.Default.GetString(sampleArray);
                return;
            }

            MessageBox.Show(res.ErrorMessage, "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        private async void button2_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog dialog = new OpenFileDialog() { Filter = "All files|*.*" })
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        await SaveFile(dialog.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task SaveFile(string filename)
        {
            using (var fileStream = File.Open(filename, FileMode.Open))
            {
                var res = await dataRepository.SendFileAsync(fileStream, filename);
                if (res.IsSuccessful)
                {
                    //Todo: Refresh logic
                    MessageBox.Show("Fájl feltöltésre került", "Feltöltve", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                MessageBox.Show(res.ErrorMessage, "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            var res = await dataRepository.ListFilesAsync();
            if(res.IsSuccessful)
            {
                dataGridView1.DataSource = res.Result?.Select(r => new {files = r}).ToList();
                return;
            }

            MessageBox.Show(res.ErrorMessage, "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewTextBoxCell cell = (DataGridViewTextBoxCell)
            dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
            textBox1.Text = cell.Value.ToString();
        }
    }
}