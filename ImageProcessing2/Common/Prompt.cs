using System.Windows.Forms;

namespace ImageProcessing2.Common
{
    public static class Prompt
    {
        public static string ShowDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 300,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 15, Top = 20, Text = text };
            TextBox textBox = new TextBox() { Left = 15, Top = 50, Width = 250 };

            Button confirmation = new Button() { Text = "Ok", Left = 155, Width = 55, Top = 80, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };

            Button cancel = new Button() { Text = "Cancel", Left = 210, Width = 55, Top = 80, DialogResult = DialogResult.Cancel };
            cancel.Click += (sender, e) => { prompt.Close(); };

            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(cancel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }
}