namespace WebsitePanel.Setup
{
	partial class ServerPasswordPage
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.lblConfirmPassword = new System.Windows.Forms.Label();
			this.txtConfirmPassword = new System.Windows.Forms.TextBox();
			this.lblPassword = new System.Windows.Forms.Label();
			this.txtPassword = new System.Windows.Forms.TextBox();
			this.lblIntro = new System.Windows.Forms.Label();
			this.chkChangePassword = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// lblConfirmPassword
			// 
			this.lblConfirmPassword.Location = new System.Drawing.Point(53, 124);
			this.lblConfirmPassword.Name = "lblConfirmPassword";
			this.lblConfirmPassword.Size = new System.Drawing.Size(106, 23);
			this.lblConfirmPassword.TabIndex = 4;
			this.lblConfirmPassword.Text = "Confirm password:";
			// 
			// txtConfirmPassword
			// 
			this.txtConfirmPassword.Location = new System.Drawing.Point(165, 124);
			this.txtConfirmPassword.Name = "txtConfirmPassword";
			this.txtConfirmPassword.PasswordChar = '*';
			this.txtConfirmPassword.Size = new System.Drawing.Size(170, 20);
			this.txtConfirmPassword.TabIndex = 5;
			// 
			// lblPassword
			// 
			this.lblPassword.Location = new System.Drawing.Point(53, 92);
			this.lblPassword.Name = "lblPassword";
			this.lblPassword.Size = new System.Drawing.Size(106, 23);
			this.lblPassword.TabIndex = 2;
			this.lblPassword.Text = "Password:";
			// 
			// txtPassword
			// 
			this.txtPassword.Location = new System.Drawing.Point(165, 92);
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.PasswordChar = '*';
			this.txtPassword.Size = new System.Drawing.Size(170, 20);
			this.txtPassword.TabIndex = 3;
			// 
			// lblIntro
			// 
			this.lblIntro.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lblIntro.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lblIntro.Location = new System.Drawing.Point(0, 0);
			this.lblIntro.Name = "lblIntro";
			this.lblIntro.Size = new System.Drawing.Size(457, 58);
			this.lblIntro.TabIndex = 0;
			this.lblIntro.Text = "Please, specify a password which will be used to access this Server from the Ente" +
				"rprise Server component. Click Next to continue.";
			// 
			// chkChangePassword
			// 
			this.chkChangePassword.Checked = true;
			this.chkChangePassword.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkChangePassword.Location = new System.Drawing.Point(56, 52);
			this.chkChangePassword.Name = "chkChangePassword";
			this.chkChangePassword.Size = new System.Drawing.Size(279, 25);
			this.chkChangePassword.TabIndex = 1;
			this.chkChangePassword.Text = "Reset Server Password";
			this.chkChangePassword.UseVisualStyleBackColor = true;
			this.chkChangePassword.CheckedChanged += new System.EventHandler(this.OnChangePasswordChecked);
			// 
			// ServerPasswordPage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.chkChangePassword);
			this.Controls.Add(this.lblConfirmPassword);
			this.Controls.Add(this.txtConfirmPassword);
			this.Controls.Add(this.lblPassword);
			this.Controls.Add(this.txtPassword);
			this.Controls.Add(this.lblIntro);
			this.Name = "ServerPasswordPage";
			this.Size = new System.Drawing.Size(457, 228);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblConfirmPassword;
		private System.Windows.Forms.TextBox txtConfirmPassword;
		private System.Windows.Forms.Label lblPassword;
		private System.Windows.Forms.TextBox txtPassword;
		private System.Windows.Forms.Label lblIntro;
		private System.Windows.Forms.CheckBox chkChangePassword;

	}
}
