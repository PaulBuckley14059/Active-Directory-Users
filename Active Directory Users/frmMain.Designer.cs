﻿namespace Active_Directory_Users
{
  partial class frmMain
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
      this.txtOutput = new System.Windows.Forms.TextBox();
      this.cmdGo = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // txtOutput
      // 
      this.txtOutput.Location = new System.Drawing.Point(602, 30);
      this.txtOutput.Multiline = true;
      this.txtOutput.Name = "txtOutput";
      this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.txtOutput.Size = new System.Drawing.Size(474, 658);
      this.txtOutput.TabIndex = 0;
      // 
      // cmdGo
      // 
      this.cmdGo.Location = new System.Drawing.Point(118, 33);
      this.cmdGo.Name = "cmdGo";
      this.cmdGo.Size = new System.Drawing.Size(131, 35);
      this.cmdGo.TabIndex = 1;
      this.cmdGo.Text = "Go!";
      this.cmdGo.UseVisualStyleBackColor = true;
      this.cmdGo.Click += new System.EventHandler(this.cmdGo_Click);
      // 
      // frmMain
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1105, 724);
      this.Controls.Add(this.cmdGo);
      this.Controls.Add(this.txtOutput);
      this.Name = "frmMain";
      this.Text = "Form1";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox txtOutput;
    private System.Windows.Forms.Button cmdGo;
  }
}

