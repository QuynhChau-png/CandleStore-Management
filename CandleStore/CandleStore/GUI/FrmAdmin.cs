using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CandleStore.BUS;
using CandleStore.DTO;
using CandleStore.Properties;

namespace CandleStore.GUI
{
    public partial class FrmAdmin : Form
    {
        private PictureBox picProduct = null!;
        private NguoiDungDTO currentAdmin;
        
        private Panel pnlSidebar = null!;
        private Panel pnlNavbar = null!;
        private Panel pnlMainContent = null!;
        
        private Button btnNavDashboard = null!, btnNavProduct = null!, btnNavCategory = null!;
        private Button btnNavCustomer = null!, btnNavInvoice = null!, btnNavUser = null!, btnLogout = null!;
        private Button currentActiveButton = null!;

        private Panel pnlDashboardView = null!;
        private Panel pnlProductView = null!;
        private Panel pnlCategoryView = null!;
        private Panel pnlCustomerView = null!;
        private Panel pnlInvoiceView = null!;
        private Panel pnlUserView = null!;

        private ComboBox cbFilterCategory = null!;

        // Tab 1: Sản phẩm
        private DataGridView dgvProducts = null!;
        private TextBox txtMaSP = null!, txtTenSP = null!, txtGia = null!, txtMuiHuong = null!, txtSoLuong = null!;
        private ComboBox cbProdCategory = null!;
        private Button btnAddProd = null!, btnEditProd = null!, btnDeleteProd = null!, btnClearProd = null!;

        // Tab 2: Danh mục phân loại nến
        private DataGridView dgvCategories = null!;
        private TextBox txtMaLoai = null!, txtTenLoai = null!;
        private Button btnAddCat = null!, btnEditCat = null!, btnDeleteCat = null!;

        // Tab 3: Hồ sơ khách hàng
        private DataGridView dgvCustomers = null!;
        private TextBox txtMaKH = null!, txtTenKH = null!, txtCustPhone = null!, txtCustAddress = null!;
        private Button btnEditCust = null!, btnDeleteCust = null!;

        // Tab 4: Tài khoản nhân viên
        private DataGridView dgvUsers = null!;
        private TextBox txtMaND = null!, txtTenND = null!, txtTenDangNhap = null!, txtMatKhauND = null!;
        private ComboBox cbRole = null!;
        private Button btnAddUser = null!, btnEditUser = null!, btnDeleteUser = null!;

        // Tab 5: Nhật ký doanh thu tổng
        private DataGridView dgvAllHistory = null!;

        // Các tầng Bus Layer
        private SanPhamBUS sanPhamBUS = new SanPhamBUS();
        private NguoiDungBUS nguoiDungBUS = new NguoiDungBUS();
        private KhachHangBUS khachHangBUS = new KhachHangBUS();
        private HoaDonBUS hoaDonBUS = new HoaDonBUS();
        private LoaiSPBUS loaiSPBUS = new LoaiSPBUS();

        private readonly Color COLOR_BG_LIGHT = Color.FromArgb(255, 245, 246);
        private readonly Color COLOR_SIDEBAR = Color.FromArgb(255, 235, 238);
        private readonly Color COLOR_PRIMARY_ROSE = Color.FromArgb(230, 115, 140);
        private readonly Color COLOR_DARK_TEXT = Color.FromArgb(90, 60, 65);

        public FrmAdmin(NguoiDungDTO adminUser)
        {
            this.currentAdmin = adminUser;
            InitializeComponent();
            
            BuildAllStaticViews();
            SwitchView(pnlDashboardView, btnNavDashboard);
        }

        private void InitializeComponent()
        {
            this.Text = "CANDLE STORE - TỔNG HỆ THỐNG QUẢN TRỊ KINH DOANH";
            this.Size = new Size(1320, 780);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(1200, 720);
            this.Size = new Size(1320, 780);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Padding = new Padding(1); this.MaximizeBox = false;
            this.BackColor = COLOR_BG_LIGHT;

            pnlSidebar = new Panel { Dock = DockStyle.Left, Width = 240, BackColor = COLOR_SIDEBAR };
            var lblLogo = new Label { Text = "🕯️ CANDLE STORE", Font = new Font("Segoe UI", 15, FontStyle.Bold), ForeColor = COLOR_DARK_TEXT, Location = new Point(10, 25), Size = new Size(220, 40), TextAlign = ContentAlignment.MiddleCenter };
            pnlSidebar.Controls.Add(lblLogo);

            int startY = 90, gap = 46, btnW = 220, btnH = 38;
            btnNavDashboard = CreateSidebarButton("  🏠  Trang chủ tổng quan", startY, btnW, btnH);
            btnNavProduct = CreateSidebarButton("  📦  Quản lý hũ nến", startY + gap, btnW, btnH);
            btnNavCategory = CreateSidebarButton("  🏷️  Danh mục phân loại", startY + gap * 2, btnW, btnH);
            btnNavCustomer = CreateSidebarButton("  👥  Hồ sơ khách hàng", startY + gap * 3, btnW, btnH);
            btnNavInvoice = CreateSidebarButton("  🧾  Nhật ký hóa đơn tổng", startY + gap * 4, btnW, btnH);
            btnNavUser = CreateSidebarButton("  🪪  Cấp tài khoản ", startY + gap * 5, btnW, btnH);

            
            btnLogout = new Button { 
                Text = "  🚪  Đăng xuất ", Location = new Point(10, 680), Size = new Size(btnW, btnH),
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.IndianRed, BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft, Cursor = Cursors.Hand
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Click += (s, e) =>
            {
                this.Hide();

                FrmLogin login = new FrmLogin();
                login.ShowDialog();

                this.Close();
            };

            pnlSidebar.Controls.AddRange(new Control[] { btnNavDashboard, btnNavProduct, btnNavCategory, btnNavCustomer, btnNavInvoice, btnNavUser, btnLogout });

            pnlNavbar = new Panel { Dock = DockStyle.Top, Height = 65, BackColor = Color.White };
            var lblGreeting = new Label { Text = $"Xin chào, {currentAdmin.TenNguoiDung}! (Tổng Quản Trị)", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = COLOR_DARK_TEXT, Location = new Point(20, 20), Size = new Size(450, 25) };
            var lblTime = new Label { Text = DateTime.Now.ToString("HH:mm:ss - dd/MM/yyyy"), Font = new Font("Segoe UI", 10, FontStyle.Italic), ForeColor = Color.Gray, Location = new Point(840, 22), Size = new Size(220, 25), Anchor = AnchorStyles.Top | AnchorStyles.Right };
            pnlNavbar.Controls.AddRange(new Control[] { lblGreeting, lblTime });

            pnlMainContent = new Panel { Dock = DockStyle.Fill, BackColor = COLOR_BG_LIGHT, Padding = new Padding(20) };

            btnNavDashboard.Click += (s, e) => { SwitchView(pnlDashboardView, btnNavDashboard); };
            btnNavProduct.Click += (s, e) => SwitchView(pnlProductView, btnNavProduct);
            btnNavCategory.Click += (s, e) => SwitchView(pnlCategoryView, btnNavCategory);
            btnNavCustomer.Click += (s, e) => SwitchView(pnlCustomerView, btnNavCustomer);
            btnNavInvoice.Click += (s, e) => SwitchView(pnlInvoiceView, btnNavInvoice);
            btnNavUser.Click += (s, e) => SwitchView(pnlUserView, btnNavUser);

            this.Controls.Add(pnlMainContent);
            this.Controls.Add(pnlNavbar);
            this.Controls.Add(pnlSidebar);
            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 25, 25));

        }
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
(
    int nLeftRect,
    int nTopRect,
    int nRightRect,
    int nBottomRect,
    int nWidthEllipse,
    int nHeightEllipse
);

        private Button CreateSidebarButton(string text, int y, int w, int h)
        {
            var btn = new Button {
                Text = text, Location = new Point(10, y), Size = new Size(w, h),
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = COLOR_DARK_TEXT, BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft, Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            
            btn.MouseEnter += (s, e) =>
            {
                if (btn != currentActiveButton)
                    btn.BackColor = Color.FromArgb(55, 56, 60);
            };

            btn.MouseLeave += (s, e) =>
            {
                if (btn != currentActiveButton)
                    btn.BackColor = Color.Transparent;
            };

            return btn;
        }

        private void BuildAllStaticViews()
        {
            pnlDashboardView = CreateDashboardPanel();
            pnlProductView = CreateProductPanel();
            pnlCategoryView = CreateCategoryPanel();
            pnlCustomerView = CreateCustomerPanel();
            pnlInvoiceView = CreateInvoicePanel();
            pnlUserView = CreateUserPanel();
        }

        private void SwitchView(Panel staticPanel, Button clickedButton)
        {
            if (currentActiveButton != null) {
                currentActiveButton.BackColor = Color.Transparent;
                currentActiveButton.ForeColor = COLOR_DARK_TEXT;
            }
            clickedButton.BackColor = COLOR_PRIMARY_ROSE; clickedButton.ForeColor = Color.White;
            currentActiveButton = clickedButton;

            pnlMainContent.Controls.Clear();
            staticPanel.Dock = DockStyle.Fill;
            pnlMainContent.Controls.Add(staticPanel);

            // Đồng bộ dữ liệu lên GUI
            if (staticPanel == pnlDashboardView) RefreshLiveKpiData();
            else if (staticPanel == pnlProductView) { LoadComboBoxCategories(); LoadAdminProductGrid(); }
            else if (staticPanel == pnlCategoryView) LoadAdminCategoryGrid();
            else if (staticPanel == pnlCustomerView) LoadAdminCustomerGrid();
            else if (staticPanel == pnlUserView) LoadAdminUserGrid();
            else if (staticPanel == pnlInvoiceView) LoadAllSystemInvoices(); // Đã thêm hàm phụ trợ ở cuối file
        }

        private void StyleGrid(DataGridView dgv)
        {
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = COLOR_PRIMARY_ROSE;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 35;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 215, 225);
            dgv.DefaultCellStyle.SelectionForeColor = COLOR_DARK_TEXT;
            dgv.RowTemplate.Height = 32; dgv.BorderStyle = BorderStyle.None; dgv.BackgroundColor = Color.White;
        }

        // ==================== Tổng quan ====================
        private Label lblKpiProd = null!, lblKpiInv = null!, lblKpiRev = null!, lblKpiCust = null!;
        private Panel pnlRealChart = null!;

        private Panel CreateDashboardPanel()
        {
            var pnl = new Panel { BackColor = COLOR_BG_LIGHT };
            int cardW = 240, cardH = 90, startX = 10, gap = 15, cardY = 20;
            
            pnl.Controls.Add(CreateKpiCardInstance("📦 TỔNG MẪU NẾN TRONG KHO", "0 hũ", "Bảng SanPham", startX, cardY, cardW, cardH, Color.LightSkyBlue, out lblKpiProd));
            pnl.Controls.Add(CreateKpiCardInstance("🛒 TỔNG ĐƠN ĐÃ XUẤT", "0 đơn", "Bảng HoaDon", startX + (cardW + gap), cardY, cardW, cardH, Color.Khaki, out lblKpiInv));
            pnl.Controls.Add(CreateKpiCardInstance("💰 DOANH THU HÔM NAY", "0 đ", "Hóa đơn trong ngày", startX + (cardW + gap) * 2, cardY, cardW, cardH, Color.LightGreen, out lblKpiRev));
            pnl.Controls.Add(CreateKpiCardInstance("👥 KHÁCH HÀNG ĐĂNG KÝ", "0 người", "Bảng KhachHang", startX + (cardW + gap) * 3, cardY, cardW, cardH, Color.LightPink, out lblKpiCust));

            var gbChart = new GroupBox { Text = "📈 Biểu đồ biến động doanh thu thực tế (VND)", Location = new Point(10, 135), Size = new Size(1005, 390), Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = COLOR_DARK_TEXT };
            pnlRealChart = new Panel { Location = new Point(20, 30), Size = new Size(965, 330), BackColor = Color.White };
            pnlRealChart.Paint += PnlRealChart_Paint;
            gbChart.Controls.Add(pnlRealChart); pnl.Controls.Add(gbChart);

            return pnl;
        }

        private Panel CreateKpiCardInstance(string title, string defVal, string sub, int x, int y, int w, int h, Color color, out Label dynamicLabel)
        {
            var card = new Panel { Location = new Point(x, y), Size = new Size(w, h), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            var lblT = new Label { Text = title, Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), ForeColor = Color.DimGray, Location = new Point(10, 8), Size = new Size(220, 20) };
            dynamicLabel = new Label { Text = defVal, Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = COLOR_DARK_TEXT, Location = new Point(10, 28), Size = new Size(220, 30) };
            var lblS = new Label { Text = sub, Font = new Font("Segoe UI", 8, FontStyle.Italic), ForeColor = Color.Gray, Location = new Point(10, 62), Size = new Size(150, 20) };
            var pnlBadge = new Panel { Location = new Point(w - 15, 0), Size = new Size(15, h), BackColor = color, Dock = DockStyle.Right };
            card.Controls.AddRange(new Control[] { lblT, dynamicLabel, lblS, pnlBadge });
            return card;
        }

        private void RefreshLiveKpiData()
        {
            var res = hoaDonBUS.GetDashboardStatistics();
            if (res.IsSuccess && res.Data != null) {
                lblKpiProd.Text = $"{res.Data["TotalProducts"]} hũ nến";
                lblKpiInv.Text = $"{res.Data["TotalInvoices"]} đơn hàng";
                lblKpiRev.Text = $"{Convert.ToDecimal(res.Data["TodayRevenue"]):#,##0} đ";
                lblKpiCust.Text = $"{res.Data["TotalCustomers"]} thành viên";
                pnlRealChart.Invalidate();
            }
        }

        private void PnlRealChart_Paint(object? sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics; g.Clear(Color.White);
            Pen axisPen = new Pen(Color.LightGray, 1);
            for (int i = 1; i <= 5; i++) g.DrawLine(axisPen, 0, i * 55, 965, i * 55);

            var res = hoaDonBUS.GetRevenueByDayStatistics();
            if (!res.IsSuccess || res.Data == null || res.Data.Rows.Count == 0) {
                g.DrawString("Chưa có hoặc lỗi dữ liệu lịch sử từ Database để vẽ đồ thị thật.", new Font("Segoe UI", 11), Brushes.Gray, 300, 130);
                return;
            }

            DataTable dt = res.Data; List<Point> points = new List<Point>();
            int stepX = 850 / Math.Max(1, dt.Rows.Count - 1);
            decimal maxRev = 200000;
            foreach (DataRow r in dt.Rows) { decimal v = Convert.ToDecimal(r["DoanhThu"]); if (v > maxRev) maxRev = v; }

            for (int i = 0; i < dt.Rows.Count; i++) {
                int x = 50 + i * stepX;
                decimal rev = Convert.ToDecimal(dt.Rows[i]["DoanhThu"]);
                int y = 260 - (int)((rev / maxRev) * 200);
                points.Add(new Point(x, y));

                DateTime date = Convert.ToDateTime(dt.Rows[i]["Ngay"]);
                g.DrawString(date.ToString("dd/MM"), this.Font, Brushes.DimGray, x - 15, 280);
                g.FillEllipse(Brushes.DeepPink, x - 4, y - 4, 8, 8);
                if (rev > 0) g.DrawString($"{rev:#,##0}", new Font("Segoe UI", 8), Brushes.DarkSlateGray, x - 20, y - 20);
            }
            if (points.Count > 1) g.DrawLines(new Pen(COLOR_PRIMARY_ROSE, 3), points.ToArray());
        }

        // ==================== Quản lý sp ====================
        private Panel CreateProductPanel()
        {
            var pnl = new Panel { BackColor = Color.White };
            var gbForm = new GroupBox { Text = "Thông tin hũ nến thơm", Location = new Point(10, 10), Size = new Size(320, 520), Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = COLOR_DARK_TEXT };
            int formX = 15, inputX = 125, startY = 35, gap = 40, txtW = 175;

            var l1 = new Label { Text = "Mã SP:", Location = new Point(formX, startY), Size = new Size(100, 25) };
            txtMaSP = new TextBox { Location = new Point(inputX, startY - 2), Size = new Size(txtW, 25), ReadOnly = true, BackColor = Color.LightGray };
            var l2 = new Label { Text = "Tên nến thơm:", Location = new Point(formX, startY + gap), Size = new Size(100, 25) };
            txtTenSP = new TextBox { Location = new Point(inputX, startY + gap - 2), Size = new Size(txtW, 25), BorderStyle = BorderStyle.FixedSingle };
            var l3 = new Label { Text = "Dòng PL:", Location = new Point(formX, startY + gap * 2), Size = new Size(100, 25) };
            cbProdCategory = new ComboBox { Location = new Point(inputX, startY + gap * 2 - 2), Size = new Size(txtW, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            var l4 = new Label { Text = "Giá bán (VND):", Location = new Point(formX, startY + gap * 3), Size = new Size(100, 25) };
            txtGia = new TextBox { Location = new Point(inputX, startY + gap * 3 - 2), Size = new Size(txtW, 25), BorderStyle = BorderStyle.FixedSingle };
            var l5 = new Label { Text = "Mùi hương:", Location = new Point(formX, startY + gap * 4), Size = new Size(100, 25) };
            txtMuiHuong = new TextBox { Location = new Point(inputX, startY + gap * 4 - 2), Size = new Size(txtW, 25), BorderStyle = BorderStyle.FixedSingle };
            var l6 = new Label { Text = "Số lượng kho:", Location = new Point(formX, startY + gap * 5), Size = new Size(100, 25) };
            txtSoLuong = new TextBox { Location = new Point(inputX, startY + gap * 5 - 2), Size = new Size(txtW, 25), BorderStyle = BorderStyle.FixedSingle };

            //var lImg = new Label { Text = "Hình ảnh mẫu:", Location = new Point(formX, startY + gap * 6), Size = new Size(100, 25) };
            //var pnlImgMock = new Panel { Location = new Point(inputX, startY + gap * 6), Size = new Size(75, 75), BorderStyle = BorderStyle.FixedSingle, BackColor = Color.Lavender };
            //pnlImgMock.Controls.Add(new Label { Text = "🕯️\n[IMAGE]", Font = new Font("Segoe UI", 8), Location = new Point(0, 20), Size = new Size(73, 40), TextAlign = ContentAlignment.MiddleCenter, ForeColor = Color.Gray });

            var lImg = new Label
            {
                Text = "Hình ảnh:",
                Location = new Point(formX, startY + gap * 6),
                Size = new Size(100, 25)
            };

            picProduct = new PictureBox
            {
                Location = new Point(inputX, startY + gap * 6),
                Size = new Size(150, 120),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom,

                BackColor = Color.White,
                Image = Image.FromStream(
    new MemoryStream(global::CandleStore.Properties.Resources.defaultimg)
)
            };

            // Ảnh mặc định
            //picProduct.Image = Image.FromFile(@"D:\CandleImages\default.jpg");

            btnAddProd = new Button { Text = "Thêm", Location = new Point(15, 465), Size = new Size(90, 35), BackColor = COLOR_PRIMARY_ROSE, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnEditProd = new Button { Text = "Sửa", Location = new Point(115, 465), Size = new Size(90, 35), BackColor = COLOR_PRIMARY_ROSE, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnDeleteProd = new Button { Text = "Xóa", Location = new Point(215, 465), Size = new Size(90, 35), BackColor = Color.IndianRed, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnClearProd = new Button { Text = "🔄 Clear", Location = new Point(15, 425), Size = new Size(290, 30), BackColor = Color.DarkGray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

            btnAddProd.Click += btnAddProd_Click;
            btnEditProd.Click += btnEditProd_Click;
            btnDeleteProd.Click += btnDeleteProd_Click;
            btnClearProd.Click += btnClearProd_Click;

            gbForm.Controls.AddRange(new Control[] {
    l1, txtMaSP,
    l2, txtTenSP,
    l3, cbProdCategory,
    l4, txtGia,
    l5, txtMuiHuong,
    l6, txtSoLuong,
    lImg,
    picProduct,
    btnAddProd,
    btnEditProd,
    btnDeleteProd,
    btnClearProd
});
            Panel pnlTop = new Panel
            {
                Location = new Point(345, 10),
                Size = new Size(680, 45),
                BackColor = Color.WhiteSmoke
            };

            Label lblFilter = new Label
            {
                Text = "Lọc phân loại:",
                Location = new Point(10, 12),
                Size = new Size(115, 25)
            };

            cbFilterCategory = new ComboBox
            {
                Location = new Point(120, 8),
                Size = new Size(240, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            dgvProducts = new DataGridView
            {
                Location = new Point(345, 60),
                Size = new Size(680, 470),
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            StyleGrid(dgvProducts);

            dgvProducts.CellClick += dgvProducts_CellClick;

            cbFilterCategory.SelectedIndexChanged += cbFilterCategory_SelectedIndexChanged;

            pnlTop.Controls.Add(lblFilter);
            pnlTop.Controls.Add(cbFilterCategory);

            pnl.Controls.Add(gbForm);
            pnl.Controls.Add(pnlTop);
            pnl.Controls.Add(dgvProducts);

            return pnl;
        }

        private void LoadComboBoxCategories()
        {
            var res = loaiSPBUS.GetAllCategories();

            if (res.IsSuccess && res.Data != null)
            {
                // Combo thêm/sửa sản phẩm
                cbProdCategory.DataSource = null;
                cbProdCategory.DataSource = res.Data;
                cbProdCategory.DisplayMember = "TenLoai";
                cbProdCategory.ValueMember = "MaLoai";

                // Combo lọc
                List<LoaiSPDTO> dsLoai = new List<LoaiSPDTO>();

                // thêm dòng "Tất cả"
                dsLoai.Add(new LoaiSPDTO
                {
                    MaLoai = 0,
                    TenLoai = "Tất cả"
                });

                // thêm dữ liệu thật
                dsLoai.AddRange(res.Data);

                cbFilterCategory.DataSource = null;
                cbFilterCategory.DataSource = dsLoai;
                cbFilterCategory.DisplayMember = "TenLoai";
                cbFilterCategory.ValueMember = "MaLoai";
            }
            else
            {
                MessageBox.Show($"[LỖI DB PHÂN LOẠI]: {res.Message}");
            }
        }
        private void LoadAdminProductGrid() {
            dgvProducts.CellFormatting += dgvProducts_CellFormatting;
            var res = sanPhamBUS.GetAllProducts();
            if (res.IsSuccess && res.Data != null)
            {
                dgvProducts.DataSource = res.Data;
                if (dgvProducts.Columns["MaSP"] != null) dgvProducts.Columns["MaSP"].HeaderText = "Mã";
                if (dgvProducts.Columns["TenSP"] != null) dgvProducts.Columns["TenSP"].HeaderText = "Tên Sản Phẩm";
                if (dgvProducts.Columns["Gia"] != null) { dgvProducts.Columns["Gia"].HeaderText = "Giá"; dgvProducts.Columns["Gia"].DefaultCellStyle.Format = "#,##0"; }
                if (dgvProducts.Columns["MaLoai"] != null) dgvProducts.Columns["MaLoai"].Visible = false;
                if (dgvProducts.Columns["TenLoai"] != null) dgvProducts.Columns["TenLoai"].HeaderText = "Dòng Phân Loại";
            }
            else
                MessageBox.Show($"[LỖI DB SẢN PHẨM]: {res.Message}", "Cảnh Báo DAO", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

        private void cbFilterCategory_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cbFilterCategory.SelectedValue == null)
                return;

            // tránh lỗi DataRowView
            if (!int.TryParse(cbFilterCategory.SelectedValue.ToString(), out int maLoai))
                return;

            var res = sanPhamBUS.GetAllProducts();

            if (res.IsSuccess && res.Data != null)
            {
                List<SanPhamDTO> ds = res.Data;

                if (maLoai != 0)
                {
                    dgvProducts.DataSource = ds
                        .Where(x => x.MaLoai == maLoai)
                        .ToList();
                }
                else
                {
                    dgvProducts.DataSource = ds;
                }
            }
        }
        private void dgvProducts_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.Value == null)
                return;

            if (dgvProducts.Columns[e.ColumnIndex]?.Name == "SoLuongTon")
            {
                int sl = Convert.ToInt32(e.Value);

                if (sl == 0)
                    e.CellStyle!.BackColor = Color.Orange;

                else if (sl < 10)
                    e.CellStyle!.BackColor = Color.Khaki;

                else
                    e.CellStyle!.BackColor = Color.LightGreen;
            }
                }
        

        private void dgvProducts_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow r = dgvProducts.Rows[e.RowIndex];
                txtMaSP.Text = r.Cells["MaSP"].Value?.ToString(); txtTenSP.Text = r.Cells["TenSP"].Value?.ToString();
                txtGia.Text = Convert.ToDecimal(r.Cells["Gia"].Value).ToString("G0");
                txtMuiHuong.Text = r.Cells["MuiHuong"].Value?.ToString(); txtSoLuong.Text = r.Cells["SoLuongTon"].Value?.ToString();
                if (r.Cells["MaLoai"].Value != null) cbProdCategory.SelectedValue = (int)r.Cells["MaLoai"].Value;
                string tenSP = txtTenSP.Text.ToLower();

                if (tenSP.Contains("lavender"))
                    picProduct.Image = Image.FromStream(
                        new MemoryStream(Properties.Resources.lavender)
                    );
                else if (tenSP.Contains("vanilla"))
                    picProduct.Image = Image.FromStream(
    new MemoryStream(Properties.Resources.vanilla)
                    );

                else if (tenSP.Contains("gỗ thông"))
                    picProduct.Image = Image.FromStream(
    new MemoryStream(Properties.Resources.pine));

                else if (tenSP.Contains("thảo mộc"))
                    picProduct.Image = Image.FromStream(
   new MemoryStream(Properties.Resources.herbal));

                else if (tenSP.Contains("kéo"))
                    picProduct.Image = Image.FromStream(
    new MemoryStream(Properties.Resources.wickcutter));

                else if (tenSP.Contains("khay"))
                    picProduct.Image = Image.FromStream(
   new MemoryStream(Properties.Resources.tray));

                else if (tenSP.Contains("sả chanh"))
                    picProduct.Image = Image.FromStream(
   new MemoryStream(Properties.Resources.lemongrass));

                else if (tenSP.Contains("cam quế"))
                    picProduct.Image = Image.FromStream(
   new MemoryStream(Properties.Resources.orangecinnamon));

                else if (tenSP.Contains("bạc hà"))
                    picProduct.Image = Image.FromStream(
   new MemoryStream(Properties.Resources.mint));

                else if (tenSP.Contains("relax"))
                    picProduct.Image = Image.FromStream(
   new MemoryStream(Properties.Resources.relaxcombo));

                else if (tenSP.Contains("sinh nhật"))
                    picProduct.Image = Image.FromStream(
   new MemoryStream(Properties.Resources.birthdaycombo));

                else if (tenSP.Contains("premium"))
                    picProduct.Image = Image.FromStream(
   new MemoryStream(Properties.Resources.premiumcombo));

                else if (tenSP.Contains("galaxy"))
                    picProduct.Image = Image.FromStream(
   new MemoryStream(Properties.Resources.galaxy));

                else if (tenSP.Contains("hoa hồng"))
                    picProduct.Image = Image.FromStream(
   new MemoryStream(Properties.Resources.rose));

                else if (tenSP.Contains("thiên nga"))
                    picProduct.Image = Image.FromStream(
   new MemoryStream(Properties.Resources.swan));

                else
                {
                    picProduct.Image = Image.FromStream(
                        new MemoryStream(Properties.Resources.defaultimg)
                    );
                }
            }
        }


        private void btnAddProd_Click(object? sender, EventArgs e) {
            if (!decimal.TryParse(txtGia.Text.Trim(), out decimal gia) || !int.TryParse(txtSoLuong.Text.Trim(), out int soLuong) || cbProdCategory.SelectedValue == null) return;
            var res = sanPhamBUS.AddProduct(new SanPhamDTO { TenSP = txtTenSP.Text.Trim(), Gia = gia, MuiHuong = txtMuiHuong.Text.Trim(), SoLuongTon = soLuong, MaLoai = (int)cbProdCategory.SelectedValue }, currentAdmin.VaiTro);
            MessageBox.Show(res.Message); if (res.IsSuccess) LoadAdminProductGrid();
        }

        private void btnEditProd_Click(object? sender, EventArgs e) {
            if (string.IsNullOrEmpty(txtMaSP.Text) || !decimal.TryParse(txtGia.Text.Trim(), out decimal gia) || !int.TryParse(txtSoLuong.Text.Trim(), out int soLuong) || cbProdCategory.SelectedValue == null) return;
            var res = sanPhamBUS.EditProduct(new SanPhamDTO { MaSP = int.Parse(txtMaSP.Text), TenSP = txtTenSP.Text.Trim(), Gia = gia, MuiHuong = txtMuiHuong.Text.Trim(), SoLuongTon = soLuong, MaLoai = (int)cbProdCategory.SelectedValue }, currentAdmin.VaiTro);
            MessageBox.Show(res.Message); if (res.IsSuccess) LoadAdminProductGrid();
        }

        private void btnDeleteProd_Click(object? sender, EventArgs e) {
            if (string.IsNullOrEmpty(txtMaSP.Text)) return;
            var res = sanPhamBUS.DeleteProduct(int.Parse(txtMaSP.Text), currentAdmin.VaiTro);
            MessageBox.Show(res.Message); if (res.IsSuccess) LoadAdminProductGrid();
        }
        private void btnClearProd_Click(object? sender, EventArgs e)
        {
            txtMaSP.Clear();
            txtTenSP.Clear();
            txtGia.Clear();
            txtMuiHuong.Clear();
            txtSoLuong.Clear();

            cbProdCategory.SelectedIndex = -1;
            dgvProducts.ClearSelection();

            picProduct.Image = Image.FromStream(
                new MemoryStream(Properties.Resources.defaultimg)
            );
        }

        // ==================== Phân loại ====================
        private Panel CreateCategoryPanel()
        {
            var pnl = new Panel { BackColor = Color.White };
            var gbForm = new GroupBox { Text = "Thông tin phân loại", Location = new Point(10, 10), Size = new Size(320, 520), Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = COLOR_DARK_TEXT };

            var l1 = new Label { Text = "Mã DM:", Location = new Point(15, 40), Size = new Size(100, 25) };
            txtMaLoai = new TextBox { Location = new Point(120, 38), Size = new Size(180, 25), ReadOnly = true, BackColor = Color.LightGray };
            var l2 = new Label { Text = "Tên PL:", Location = new Point(15, 90), Size = new Size(100, 25) };
            txtTenLoai = new TextBox { Location = new Point(120, 88), Size = new Size(180, 25), BorderStyle = BorderStyle.FixedSingle };

            btnAddCat = new Button { Text = "Thêm mới", Location = new Point(15, 150), Size = new Size(90, 35), BackColor = COLOR_PRIMARY_ROSE, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnEditCat = new Button { Text = "Đổi tên", Location = new Point(115, 150), Size = new Size(90, 35), BackColor = COLOR_PRIMARY_ROSE, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnDeleteCat = new Button { Text = "Xóa bỏ", Location = new Point(215, 150), Size = new Size(90, 35), BackColor = Color.IndianRed, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            gbForm.Controls.AddRange(new Control[] { l1, txtMaLoai, l2, txtTenLoai, btnAddCat, btnEditCat, btnDeleteCat });

            dgvCategories = new DataGridView { Location = new Point(345, 20), Size = new Size(680, 510), AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, ReadOnly = true, RowHeadersVisible = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            StyleGrid(dgvCategories);

            dgvCategories.CellClick += (s, e) => {
                if (e.RowIndex >= 0) {
                    txtMaLoai.Text = dgvCategories.Rows[e.RowIndex].Cells["MaLoai"].Value?.ToString();
                    txtTenLoai.Text = dgvCategories.Rows[e.RowIndex].Cells["TenLoai"].Value?.ToString();
                }
            };
            btnAddCat.Click += (s, e) => {
                var res = loaiSPBUS.AddCategory(new LoaiSPDTO { TenLoai = txtTenLoai.Text.Trim() });
                MessageBox.Show(res.Message); if (res.IsSuccess) { LoadAdminCategoryGrid(); LoadComboBoxCategories(); }
            };
            btnEditCat.Click += (s, e) => {
                if (string.IsNullOrEmpty(txtMaLoai.Text)) return;
                var res = loaiSPBUS.EditCategory(new LoaiSPDTO { MaLoai = int.Parse(txtMaLoai.Text), TenLoai = txtTenLoai.Text.Trim() });
                MessageBox.Show(res.Message); if (res.IsSuccess) { LoadAdminCategoryGrid(); LoadComboBoxCategories(); }
            };
            btnDeleteCat.Click += (s, e) => {
                if (string.IsNullOrEmpty(txtMaLoai.Text)) return;
                var res = loaiSPBUS.DeleteCategory(int.Parse(txtMaLoai.Text));
                MessageBox.Show(res.Message); if (res.IsSuccess) { LoadAdminCategoryGrid(); LoadComboBoxCategories(); }
            };

            pnl.Controls.AddRange(new Control[] { gbForm, dgvCategories });
            return pnl;
        }

        private void LoadAdminCategoryGrid() { 
            var res = loaiSPBUS.GetAllCategories(); 
            if (res.IsSuccess && res.Data != null) {
                dgvCategories.DataSource = res.Data; 
            } else {
                MessageBox.Show($"[LỖI DB DANH MỤC]: {res.Message}", "Cảnh Báo DAO", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==================== Hồ sơ khách hàng ====================
        private Panel CreateCustomerPanel()
        {
            var pnl = new Panel { BackColor = Color.White };
            var gbForm = new GroupBox { Text = "Thông tin thành viên tiệm nến", Location = new Point(10, 10), Size = new Size(320, 520), Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = COLOR_DARK_TEXT };
            int formX = 15, inputX = 120, startY = 40, gap = 50, txtW = 180;

            var l1 = new Label { Text = "Mã KH:", Location = new Point(formX, startY), Size = new Size(100, 25) };
            txtMaKH = new TextBox { Location = new Point(inputX, startY - 2), Size = new Size(txtW, 25), ReadOnly = true, BackColor = Color.LightGray };
            var l2 = new Label { Text = "Họ và tên:", Location = new Point(formX, startY + gap), Size = new Size(100, 25) };
            txtTenKH = new TextBox { Location = new Point(inputX, startY + gap - 2), Size = new Size(txtW, 25), BorderStyle = BorderStyle.FixedSingle };
            var l3 = new Label { Text = "SĐT:", Location = new Point(formX, startY + gap * 2), Size = new Size(100, 25) };
            txtCustPhone = new TextBox { Location = new Point(inputX, startY + gap * 2 - 2), Size = new Size(txtW, 25), BorderStyle = BorderStyle.FixedSingle };
            var l4 = new Label { Text = "Địa chỉ nhà:", Location = new Point(formX, startY + gap * 3), Size = new Size(100, 25) };
            txtCustAddress = new TextBox { Location = new Point(inputX, startY + gap * 3 - 2), Size = new Size(txtW, 25), BorderStyle = BorderStyle.FixedSingle };

            btnEditCust = new Button { Text = "Sửa Thông Tin", Location = new Point(20, 260), Size = new Size(280, 35), BackColor = COLOR_PRIMARY_ROSE, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnDeleteCust = new Button { Text = "XÓA KHỎI HỆ THỐNG", Location = new Point(20, 310), Size = new Size(280, 35), BackColor = Color.IndianRed, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            gbForm.Controls.AddRange(new Control[] { l1, txtMaKH, l2, txtTenKH, l3, txtCustPhone, l4, txtCustAddress, btnEditCust, btnDeleteCust });

            dgvCustomers = new DataGridView { Location = new Point(345, 20), Size = new Size(670, 510), AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, ReadOnly = true, RowHeadersVisible = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            StyleGrid(dgvCustomers);
            
            dgvCustomers.CellClick += dgvCustomers_CellClick;
            btnEditCust.Click += btnEditCust_Click; btnDeleteCust.Click += btnDeleteCust_Click;

            pnl.Controls.AddRange(new Control[] { gbForm, dgvCustomers });
            return pnl;
        }

        private void LoadAdminCustomerGrid() { var res = khachHangBUS.GetAllCustomers(); if (res.IsSuccess && res.Data != null) dgvCustomers.DataSource = res.Data; }
        
        private void dgvCustomers_CellClick(object? sender, DataGridViewCellEventArgs e) {
            if (e.RowIndex >= 0) {
                DataGridViewRow r = dgvCustomers.Rows[e.RowIndex];
                txtMaKH.Text = r.Cells["MaKH"].Value?.ToString(); txtTenKH.Text = r.Cells["TenKH"].Value?.ToString();
                txtCustPhone.Text = r.Cells["SDT"].Value?.ToString(); txtCustAddress.Text = r.Cells["DiaChi"].Value?.ToString();
            }
        }
        private void btnEditCust_Click(object? sender, EventArgs e) {
            if (string.IsNullOrEmpty(txtMaKH.Text)) return;
            var res = khachHangBUS.EditCustomer(new KhachHangDTO { MaKH = int.Parse(txtMaKH.Text), TenKH = txtTenKH.Text.Trim(), SDT = txtCustPhone.Text.Trim(), DiaChi = txtCustAddress.Text.Trim() });
            MessageBox.Show(res.Message); if (res.IsSuccess) LoadAdminCustomerGrid();
        }
        private void btnDeleteCust_Click(object? sender, EventArgs e) {
            if (string.IsNullOrEmpty(txtMaKH.Text)) return;
            var res = khachHangBUS.DeleteCustomer(int.Parse(txtMaKH.Text));
            MessageBox.Show(res.Message); if (res.IsSuccess) LoadAdminCustomerGrid();
        }

        // ==================== Cấp tài khoản ====================
        private Panel CreateUserPanel()
        {
            var pnl = new Panel { BackColor = Color.White };
            var gbForm = new GroupBox { Text = "Cấp mới & sửa đổi quyền nhân sự", Location = new Point(10, 10), Size = new Size(320, 520), Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = COLOR_DARK_TEXT };
            int formX = 15, inputX = 125, startY = 40, gap = 45, txtW = 175;

            var l1 = new Label { Text = "Mã NV:", Location = new Point(formX, startY), Size = new Size(100, 25) };
            txtMaND = new TextBox { Location = new Point(inputX, startY - 2), Size = new Size(txtW, 25), ReadOnly = true, BackColor = Color.LightGray };
            var l2 = new Label { Text = "Họ và tên:", Location = new Point(formX, startY + gap), Size = new Size(100, 25) };
            txtTenND = new TextBox { Location = new Point(inputX, startY + gap - 2), Size = new Size(txtW, 25), BorderStyle = BorderStyle.FixedSingle };
            var l3 = new Label { Text = "Tên ĐN:", Location = new Point(formX, startY + gap * 2), Size = new Size(100, 25) };
            txtTenDangNhap = new TextBox { Location = new Point(inputX, startY + gap * 2 - 2), Size = new Size(txtW, 25), BorderStyle = BorderStyle.FixedSingle };
            
            var lPass = new Label { Text = "Mật khẩu tạo:", Location = new Point(formX, startY + gap * 3), Size = new Size(100, 25) };
            txtMatKhauND = new TextBox { Location = new Point(inputX, startY + gap * 3 - 2), Size = new Size(txtW, 25), PasswordChar = '•', BorderStyle = BorderStyle.FixedSingle };

            var l4 = new Label { Text = "Quyền hạn:", Location = new Point(formX, startY + gap * 4), Size = new Size(100, 25) };
            cbRole = new ComboBox { Location = new Point(inputX, startY + gap * 4 - 2), Size = new Size(txtW, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cbRole.Items.AddRange(new string[] { "staff", "admin" }); cbRole.SelectedIndex = 0;

            btnAddUser = new Button { Text = "➕ Thêm mới nhân viên", Location = new Point(20, 270), Size = new Size(280, 35), BackColor = Color.MediumSeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnEditUser = new Button { Text = "Cập Nhật Thông Tin", Location = new Point(20, 315), Size = new Size(280, 35), BackColor = COLOR_PRIMARY_ROSE, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnDeleteUser = new Button { Text = "SA THẢI NHÂN VIÊN", Location = new Point(20, 360), Size = new Size(280, 35), BackColor = Color.IndianRed, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            
            gbForm.Controls.AddRange(new Control[] { l1, txtMaND, l2, txtTenND, l3, txtTenDangNhap, lPass, txtMatKhauND, l4, cbRole, btnAddUser, btnEditUser, btnDeleteUser });

            dgvUsers = new DataGridView { Location = new Point(345, 20), Size = new Size(670, 510), AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, ReadOnly = true, RowHeadersVisible = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            StyleGrid(dgvUsers);
            
            dgvUsers.CellClick += dgvUsers_CellClick;
            btnEditUser.Click += btnEditUser_Click; btnDeleteUser.Click += btnDeleteUser_Click;
            
            btnAddUser.Click += (s, e) => {
                NguoiDungDTO u = new NguoiDungDTO { TenNguoiDung = txtTenND.Text.Trim(), TenDangNhap = txtTenDangNhap.Text.Trim(), MatKhau = txtMatKhauND.Text.Trim(), VaiTro = cbRole.SelectedItem?.ToString() ?? "staff" };
                var res = nguoiDungBUS.Register(u, txtMatKhauND.Text.Trim());
                MessageBox.Show(res.Message); if (res.IsSuccess) { LoadAdminUserGrid(); txtMatKhauND.Clear(); }
            };

            pnl.Controls.AddRange(new Control[] { gbForm, dgvUsers });
            return pnl;
        }

        private void LoadAdminUserGrid() { var res = nguoiDungBUS.GetAllUsers(); if (res.IsSuccess && res.Data != null) dgvUsers.DataSource = res.Data; }
        
        private void dgvUsers_CellClick(object? sender, DataGridViewCellEventArgs e) {
            if (e.RowIndex >= 0) {
                DataGridViewRow r = dgvUsers.Rows[e.RowIndex];
                txtMaND.Text = r.Cells["MaND"].Value?.ToString(); txtTenND.Text = r.Cells["TenNguoiDung"].Value?.ToString();
                txtTenDangNhap.Text = r.Cells["TenDangNhap"].Value?.ToString(); cbRole.SelectedItem = r.Cells["VaiTro"].Value?.ToString();
            }
        }
        private void btnEditUser_Click(object? sender, EventArgs e) {
            if (string.IsNullOrEmpty(txtMaND.Text) || cbRole.SelectedItem == null) return;
            var res = nguoiDungBUS.EditUser(new NguoiDungDTO { MaND = int.Parse(txtMaND.Text), TenNguoiDung = txtTenND.Text.Trim(), VaiTro = cbRole.SelectedItem.ToString()! }, currentAdmin.VaiTro);
            MessageBox.Show(res.Message); if (res.IsSuccess) LoadAdminUserGrid();
        }
        private void btnDeleteUser_Click(object? sender, EventArgs e) {
            if (string.IsNullOrEmpty(txtMaND.Text)) return;
            var res = nguoiDungBUS.DeleteUser(int.Parse(txtMaND.Text), currentAdmin.VaiTro, currentAdmin.MaND);
            MessageBox.Show(res.Message); if (res.IsSuccess) LoadAdminUserGrid();
        }

        // ==================== Nhật ký hóa đơn ====================
        private Panel CreateInvoicePanel()
        {
            var pnl = new Panel { BackColor = Color.White };
            var lbl = new Label { Text = "🧾 NHẬT KÝ GIAO DỊCH VÀ DOANH SỐ TOÀN CỬA HÀNG", Location = new Point(20, 15), AutoSize = true, Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = COLOR_DARK_TEXT };
            dgvAllHistory = new DataGridView { Location = new Point(20, 55), Size = new Size(995, 475), AllowUserToAddRows = false, ReadOnly = true, RowHeadersVisible = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            StyleGrid(dgvAllHistory);
            
            pnl.Controls.AddRange(new Control[] { lbl, dgvAllHistory });
            return pnl;
        }

        private void LoadAllSystemInvoices()
        {
            var res = hoaDonBUS.GetSalesHistory(currentAdmin.MaND, currentAdmin.VaiTro);
            if (res.IsSuccess && res.Data != null) dgvAllHistory.DataSource = res.Data;
        }

        private void ClearInputs() { txtMaSP.Clear(); txtTenSP.Clear(); txtGia.Clear(); txtMuiHuong.Clear(); txtSoLuong.Clear(); }
    }
}