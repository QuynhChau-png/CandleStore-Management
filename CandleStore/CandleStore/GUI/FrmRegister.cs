using System;
using System.Drawing;
using System.Windows.Forms;
using CandleStore.BUS;
using CandleStore.DTO;

namespace CandleStore.GUI
{
    public partial class FrmRegister : Form
    {
        private Label lblTitle = null!, lblFullName = null!, lblUser = null!, lblPass = null!, lblConfirm = null!;
        private TextBox txtFullName = null!, txtUser = null!, txtPass = null!, txtConfirm = null!;
        private Button btnRegister = null!, btnBack = null!;
        private NguoiDungBUS nguoiDungBUS = new NguoiDungBUS();

        public FrmRegister()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "CandleStore - Đăng Ký Nhân Viên";
            this.Size = new Size(400, 420);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(255, 242, 244); // Hồng pastel

            lblTitle = new Label { Text = "ĐĂNG KÝ TÀI KHOẢN", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Color.FromArgb(139, 76, 92), Location = new Point(30, 20), Size = new Size(340, 35), TextAlign = ContentAlignment.MiddleCenter };

            int lblX = 40, inputX = 170, startY = 80, gap = 45, width = 180;
            Color textColor = Color.FromArgb(90, 60, 65);

            lblFullName = new Label { Text = "Họ và tên:", Location = new Point(lblX, startY), Size = new Size(110, 25), Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = textColor };
            txtFullName = new TextBox { Location = new Point(inputX, startY - 2), Size = new Size(width, 25), BorderStyle = BorderStyle.FixedSingle };

            lblUser = new Label { Text = "Tên đăng nhập:", Location = new Point(lblX, startY + gap), Size = new Size(110, 25), Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = textColor };
            txtUser = new TextBox { Location = new Point(inputX, startY + gap - 2), Size = new Size(width, 25), BorderStyle = BorderStyle.FixedSingle };

            lblPass = new Label { Text = "Mật khẩu:", Location = new Point(lblX, startY + gap * 2), Size = new Size(110, 25), Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = textColor };
            txtPass = new TextBox { Location = new Point(inputX, startY + gap * 2 - 2), Size = new Size(width, 25), PasswordChar = '•', BorderStyle = BorderStyle.FixedSingle };

            lblConfirm = new Label { Text = "Xác nhận lại:", Location = new Point(lblX, startY + gap * 3), Size = new Size(110, 25), Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = textColor };
            txtConfirm = new TextBox { Location = new Point(inputX, startY + gap * 3 - 2), Size = new Size(width, 25), PasswordChar = '•', BorderStyle = BorderStyle.FixedSingle };

            btnRegister = new Button { Text = "Tạo Tài Khoản", Location = new Point(40, 280), Size = new Size(310, 40), BackColor = Color.FromArgb(230, 115, 140), ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold), FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnRegister.FlatAppearance.BorderSize = 0;

            btnBack = new Button { Text = "Quay lại Đăng nhập", Location = new Point(40, 330), Size = new Size(310, 30), FlatStyle = FlatStyle.Flat, ForeColor = Color.FromArgb(139, 76, 92), Font = new Font("Segoe UI", 9, FontStyle.Underline), BackColor = Color.Transparent, Cursor = Cursors.Hand };
            btnBack.FlatAppearance.BorderSize = 0;

            btnRegister.Click += BtnRegister_Click;
            btnBack.Click += (s, e) => this.Close();

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblFullName); this.Controls.Add(txtFullName);
            this.Controls.Add(lblUser); this.Controls.Add(txtUser);
            this.Controls.Add(lblPass); this.Controls.Add(txtPass);
            this.Controls.Add(lblConfirm); this.Controls.Add(txtConfirm);
            this.Controls.Add(btnRegister); this.Controls.Add(btnBack);
        }

        private void BtnRegister_Click(object? sender, EventArgs e)
{
    NguoiDungDTO newUser = new NguoiDungDTO {
        TenNguoiDung = txtFullName.Text.Trim(),
        TenDangNhap = txtUser.Text.Trim(),
        MatKhau = txtPass.Text.Trim()
    };

    // Gửi xuống BUS xử lý chu trình kiểm tra nghiệp vụ khép kín
    ServiceResult result = nguoiDungBUS.Register(newUser, txtConfirm.Text.Trim());

    if (result.IsSuccess)
    {
        MessageBox.Show(result.Message, "Đăng Ký Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        this.Close(); // Đóng form đăng ký để quay lại màn hình đăng nhập
    }
    else
    {
        // Hiển thị thông báo chặn nghiệp vụ hoặc lỗi Runtime SQL tương ứng
        MessageBox.Show(result.Message, "Cảnh Báo Đăng Ký", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}
    }
}