using System;
using System.Drawing;
using System.Windows.Forms;
using CandleStore.BUS;
using CandleStore.DTO;

namespace CandleStore.GUI
{
    public partial class FrmLogin : Form
    {
        private Label lblTitle = null!, lblUsername = null!, lblPassword = null!;
        private TextBox txtUsername = null!, txtPassword = null!;
        private Button btnLogin = null!, btnGoToRegister = null!;
        private NguoiDungBUS nguoiDungBUS = new NguoiDungBUS();

        public FrmLogin()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "CandleStore - Đăng Nhập Hệ Thống";
            this.Size = new Size(400, 360);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(255, 242, 244);
            this.AcceptButton = btnLogin;

            lblTitle = new Label { Text = "✨ CANDLE STORE ✨", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.FromArgb(139, 76, 92), Location = new Point(30, 25), Size = new Size(340, 40), TextAlign = ContentAlignment.MiddleCenter };

            lblUsername = new Label { Text = "Tên đăng nhập:", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(90, 60, 65), Location = new Point(50, 90), Size = new Size(110, 25) };
            txtUsername = new TextBox { Location = new Point(170, 88), Size = new Size(180, 25), Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle };

            lblPassword = new Label { Text = "Mật khẩu:", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(90, 60, 65), Location = new Point(50, 130), Size = new Size(110, 25) };
            txtPassword = new TextBox { Location = new Point(170, 128), Size = new Size(180, 25), Font = new Font("Segoe UI", 10), PasswordChar = '•', BorderStyle = BorderStyle.FixedSingle };

            btnLogin = new Button { Text = "LOG IN", Font = new Font("Segoe UI", 11, FontStyle.Bold), Location = new Point(50, 195), Size = new Size(300, 42), BackColor = Color.FromArgb(230, 115, 140), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += BtnLogin_Click;

            btnGoToRegister = new Button { Text = "Chưa có tài khoản? Đăng ký nhân viên tại đây", Font = new Font("Segoe UI", 9, FontStyle.Underline), ForeColor = Color.FromArgb(139, 76, 92), Location = new Point(50, 255), Size = new Size(300, 30), FlatStyle = FlatStyle.Flat, BackColor = Color.Transparent, Cursor = Cursors.Hand };
            btnGoToRegister.FlatAppearance.BorderSize = 0;
            btnGoToRegister.Click += (s, e) => { FrmRegister reg = new FrmRegister(); reg.ShowDialog(); };

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblUsername); this.Controls.Add(txtUsername);
            this.Controls.Add(lblPassword); this.Controls.Add(txtPassword);
            this.Controls.Add(btnLogin); this.Controls.Add(btnGoToRegister);
        }

        private void BtnLogin_Click(object? sender, EventArgs e)
{
    string userText = txtUsername.Text.Trim();
    string passText = txtPassword.Text.Trim();

    // Gọi BUS xử lý bọc lỗi tập trung và đón nhận ServiceResult
    ServiceResult<NguoiDungDTO> result = nguoiDungBUS.Login(userText, passText);

    if (result.IsSuccess && result.Data != null)
    {
        MessageBox.Show(result.Message, "Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        this.Hide();

        // Phân chia luồng giao diện rạch ròi dựa trên dữ liệu chuẩn từ BUS trả về
        if (result.Data.VaiTro.ToLower() == "admin") {
            FrmAdmin adminForm = new FrmAdmin(result.Data);
            adminForm.ShowDialog();
        } else {
            FrmStaff staffForm = new FrmStaff(result.Data);
            staffForm.ShowDialog();
        }
        this.Close();
    }
    else
    {
        // Hiển thị trực tiếp câu chẩn đoán lỗi lịch sự do BUS phân rã dịch sẵn (Dù là gõ sai hay sập nguồn DB)
        MessageBox.Show(result.Message, "Thông Báo Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
    }
}