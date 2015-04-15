namespace SAI_NeuralNetworks
{
    partial class NeuroAssistant
    {
        /// <summary> 
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Обязательный метод для поддержки конструктора - не изменяйте 
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtLogs = new System.Windows.Forms.TextBox();
            this.StartLearn = new System.Windows.Forms.Button();
            this.txtKErr = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtKLern = new System.Windows.Forms.TextBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtLogs
            // 
            this.txtLogs.Location = new System.Drawing.Point(0, 42);
            this.txtLogs.Multiline = true;
            this.txtLogs.Name = "txtLogs";
            this.txtLogs.Size = new System.Drawing.Size(536, 314);
            this.txtLogs.TabIndex = 0;
            // 
            // StartLearn
            // 
            this.StartLearn.Location = new System.Drawing.Point(409, 3);
            this.StartLearn.Name = "StartLearn";
            this.StartLearn.Size = new System.Drawing.Size(130, 33);
            this.StartLearn.TabIndex = 1;
            this.StartLearn.Text = "Start Learn";
            this.StartLearn.UseVisualStyleBackColor = true;
            this.StartLearn.Click += new System.EventHandler(this.StartLearn_Click);
            // 
            // txtKErr
            // 
            this.txtKErr.Location = new System.Drawing.Point(154, 20);
            this.txtKErr.Margin = new System.Windows.Forms.Padding(4);
            this.txtKErr.Name = "txtKErr";
            this.txtKErr.Size = new System.Drawing.Size(164, 22);
            this.txtKErr.TabIndex = 13;
            this.txtKErr.Text = "0,1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(154, 0);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(148, 17);
            this.label4.TabIndex = 12;
            this.label4.Text = "Критерий остановки:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 0);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(140, 17);
            this.label3.TabIndex = 11;
            this.label3.Text = "Скорость обучения:";
            // 
            // txtKLern
            // 
            this.txtKLern.Location = new System.Drawing.Point(3, 20);
            this.txtKLern.Margin = new System.Windows.Forms.Padding(4);
            this.txtKLern.Name = "txtKLern";
            this.txtKLern.Size = new System.Drawing.Size(141, 22);
            this.txtKLern.TabIndex = 10;
            this.txtKLern.Text = "0,1";
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(325, 3);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(78, 33);
            this.btnStop.TabIndex = 14;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // NeuroAssistant
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.txtKErr);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtKLern);
            this.Controls.Add(this.StartLearn);
            this.Controls.Add(this.txtLogs);
            this.Name = "NeuroAssistant";
            this.Size = new System.Drawing.Size(545, 359);
            this.Load += new System.EventHandler(this.NeuroAssistant_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtLogs;
        private System.Windows.Forms.Button StartLearn;
        private System.Windows.Forms.TextBox txtKErr;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtKLern;
        private System.Windows.Forms.Button btnStop;
    }
}
