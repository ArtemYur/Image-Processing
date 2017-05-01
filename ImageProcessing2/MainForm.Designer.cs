namespace ImageProcessing2
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.glControl = new OpenTK.GLControl();
            this.brightnessDetail = new System.Windows.Forms.Label();
            this.minBrightness = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // glControl
            // 
            this.glControl.BackColor = System.Drawing.Color.Black;
            this.glControl.Location = new System.Drawing.Point(0, 0);
            this.glControl.Name = "glControl";
            this.glControl.Size = new System.Drawing.Size(256, 256);
            this.glControl.TabIndex = 0;
            this.glControl.VSync = false;
            this.glControl.Load += new System.EventHandler(this.glControl_Load);
            this.glControl.Paint += new System.Windows.Forms.PaintEventHandler(this.glControl_Paint);
            this.glControl.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.glControl_KeyPress);
            this.glControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.glControl_MouseMove);
            // 
            // brightnessDetail
            // 
            this.brightnessDetail.AutoSize = true;
            this.brightnessDetail.Location = new System.Drawing.Point(262, 9);
            this.brightnessDetail.Name = "brightnessDetail";
            this.brightnessDetail.Size = new System.Drawing.Size(0, 13);
            this.brightnessDetail.TabIndex = 1;
            // 
            // minBrightness
            // 
            this.minBrightness.Enabled = false;
            this.minBrightness.Location = new System.Drawing.Point(265, 42);
            this.minBrightness.Name = "minBrightness";
            this.minBrightness.Size = new System.Drawing.Size(100, 20);
            this.minBrightness.TabIndex = 2;
            this.minBrightness.Text = "0.6";
            // 
            // textBox1
            // 
            this.textBox1.Enabled = false;
            this.textBox1.Location = new System.Drawing.Point(265, 68);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 3;
            this.textBox1.Text = "1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(257, 256);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.minBrightness);
            this.Controls.Add(this.brightnessDetail);
            this.Controls.Add(this.glControl);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private OpenTK.GLControl glControl;
        private System.Windows.Forms.Label brightnessDetail;
        private System.Windows.Forms.TextBox minBrightness;
        private System.Windows.Forms.TextBox textBox1;
    }
}

