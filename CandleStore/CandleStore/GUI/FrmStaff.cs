using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CandleStore.BUS;
using CandleStore.DTO;

namespace CandleStore.GUI
{
    public partial class FrmStaff : Form
    {
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
        private NguoiDungDTO currentStaff;
        
        // Khung Panel nền tảng Layout Phẳng
        private Panel pnlSidebar = null!;
        private Panel pnlNavbar = null!;
        private Panel pnlMainContent = null!;
        
        // Buttons thanh điều hướng trái 
        private Button btnNavPos = null!, btnNavHistory = null!, btnLogout = null!;
        private Button currentActiveButton = null!;


        private Panel pnlPosView = null!;
        private Panel pnlHistoryView = null!;

        private TextBox txtCustPhone = null!, txtCustName = null!, txtCustAddress = null!;
        private Label lblCustStatus = null!;
        private Button btnFindCust = null!, btnResetCust = null!;

        private TextBox txtSearchProduct = null!;
        private ComboBox cbProducts = null!;
        private TextBox txtSaleQty = null!;
        private Button btnAddCart = null!, btnRemoveCart = null!, btnClearCart = null!;

        private Label lblTotalCart = null!;
        private Button btnCheckout = null!;
        private DataGridView dgvCart = null!;

        private DataGridView dgvMyHistory = null!;

        // Dữ liệu tạm
        private KhachHangDTO? activeCustomer = null;
        private List<ChiTietHoaDonDTO> currentCart = new List<ChiTietHoaDonDTO>();
        private DataTable dtCartBind = null!;
        private List<SanPhamDTO> allProductsCache = new List<SanPhamDTO>();

        // Tầng nghiệp vụ BUS
        private SanPhamBUS sanPhamBUS = new SanPhamBUS();
        private KhachHangBUS khachHangBUS = new KhachHangBUS();
        private HoaDonBUS hoaDonBUS = new HoaDonBUS();

        // Bảng định mã màu Flat UI Premium hồng nến ấm áp
        private readonly Color COLOR_BG_LIGHT = Color.FromArgb(255, 245, 246);
        private readonly Color COLOR_SIDEBAR = Color.FromArgb(255, 235, 238);
        private readonly Color COLOR_PRIMARY_ROSE = Color.FromArgb(230, 115, 140);
        private readonly Color COLOR_DARK_TEXT = Color.FromArgb(90, 60, 65);

        public FrmStaff(NguoiDungDTO staffUser)
        {
            this.currentStaff = staffUser;
            InitializeComponent();
            
            // Khởi tạo các View vào bộ nhớ đệm
            BuildAllStaticViews();
            
            // Kích hoạt View quầy bán hàng (POS) đầu tiên
            SwitchView(pnlPosView, btnNavPos);
        }

        private void InitializeComponent()
        {
            this.Text = "CANDLE STORE - HỆ THỐNG QUẦY THU NGÂN (POS)";
           
            this.Size = new Size(1300, 780);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(1200, 720);
            this.BackColor = COLOR_BG_LIGHT;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Padding = new Padding(1);

            // VẼ THANH SIDEBAR TRÁI
            pnlSidebar = new Panel { Dock = DockStyle.Left, Width = 240, BackColor = COLOR_SIDEBAR };
            var lblLogo = new Label { Text = "🕯️ CANDLE STORE", Font = new Font("Segoe UI", 15, FontStyle.Bold), ForeColor = COLOR_DARK_TEXT, Location = new Point(10, 25), Size = new Size(220, 40), TextAlign = ContentAlignment.MiddleCenter };
            pnlSidebar.Controls.Add(lblLogo);

            int startY = 90, gap = 46, btnW = 220, btnH = 38;
            btnNavPos = CreateSidebarButton("  🛒  Lập đơn bán hàng", startY, btnW, btnH);
            btnNavHistory = CreateSidebarButton("  📑  Nhật ký ca làm việc", startY + gap, btnW, btnH);
            
            btnLogout = new Button { 
                Text = "  🚪  Đăng xuất ca làm", Location = new Point(10, 680), Size = new Size(btnW, btnH),
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

            pnlSidebar.Controls.AddRange(new Control[] { btnNavPos, btnNavHistory, btnLogout });

            // VẼ NAVBAR TRÊN
            pnlNavbar = new Panel { Dock = DockStyle.Top, Height = 65, BackColor = Color.White };
            var lblGreeting = new Label { Text = $"Xin chào thu ngân, {currentStaff.TenNguoiDung}!", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = COLOR_DARK_TEXT, Location = new Point(20, 20), Size = new Size(450, 25) };
            var lblTime = new Label { Text = DateTime.Now.ToString("HH:mm:ss - dd/MM/yyyy"), Font = new Font("Segoe UI", 10, FontStyle.Italic), ForeColor = Color.Gray, Location = new Point(820, 22), Size = new Size(220, 25), Anchor = AnchorStyles.Top | AnchorStyles.Right };
            pnlNavbar.Controls.AddRange(new Control[] { lblGreeting, lblTime });

            // VÙNG KHÔNG GIAN HIỂN THỊ CHÍNH
            pnlMainContent = new Panel { Dock = DockStyle.Fill, BackColor = COLOR_BG_LIGHT, Padding = new Padding(20) };

            // Thao tác click chuyển tab
            btnNavPos.Click += (s, e) => SwitchView(pnlPosView, btnNavPos);
            btnNavHistory.Click += (s, e) => SwitchView(pnlHistoryView, btnNavHistory);

            this.Controls.Add(pnlMainContent);
            this.Controls.Add(pnlNavbar);
            this.Controls.Add(pnlSidebar);
            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 25, 25));
        }

        private Button CreateSidebarButton(string text, int y, int w, int h)
        {
            var btn = new Button {
                Text = text, Location = new Point(10, y), Size = new Size(w, h),
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = COLOR_DARK_TEXT, BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft, Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void BuildAllStaticViews()
        {
            pnlPosView = CreatePosPanel();
            pnlHistoryView = CreateHistoryPanel();
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

            // Nạp dữ liệu ca làm mới nhất nếu nhảy sang tab Lịch sử
            if (staticPanel == pnlHistoryView) {
                LoadPersonalSalesHistory();
            }
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
            dgv.RowTemplate.Height = 35; dgv.BorderStyle = BorderStyle.None; dgv.BackgroundColor = Color.White;
        }

        // ==================== 🛒 MÀN HÌNH 1: QUẦY POS BÁN HÀNG ====================
        private Panel CreatePosPanel()
        {
            var pnl = new Panel { BackColor = COLOR_BG_LIGHT };

            // 1. KHU VỰC KHÁCH HÀNG
            var gbCust = new GroupBox { Text = "1. Định danh khách hàng", Location = new Point(10, 10), Size = new Size(420, 175), Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = COLOR_DARK_TEXT, BackColor = Color.White };
            var lP = new Label { Text = "Số điện thoại:", Location = new Point(15, 35), Size = new Size(100, 25) };
            txtCustPhone = new TextBox { Location = new Point(115, 33), Size = new Size(170, 25), BorderStyle = BorderStyle.FixedSingle };
            btnFindCust = new Button { Text = "Kiểm Tra", Location = new Point(295, 31), Size = new Size(110, 28), BackColor = Color.SteelBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            
            lblCustStatus = new Label { Text = "📢 Đang đặt ở chế độ: KHÁCH VÃNG LAI", Location = new Point(15, 68), Size = new Size(390, 20), ForeColor = Color.DarkOrange, Font = new Font("Segoe UI", 8.5f, FontStyle.Bold) };
            
            var lN = new Label { Text = "Tên khách:", Location = new Point(15, 95), Size = new Size(100, 25) };
            txtCustName = new TextBox { Location = new Point(115, 93), Size = new Size(170, 25), BorderStyle = BorderStyle.FixedSingle };
            btnResetCust = new Button { Text = "Đổi Khách", Location = new Point(295, 91), Size = new Size(110, 28), BackColor = Color.Gray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            
            var lA = new Label { Text = "Địa chỉ mua:", Location = new Point(15, 135), Size = new Size(100, 25) };
            txtCustAddress = new TextBox { Location = new Point(115, 133), Size = new Size(290, 25), BorderStyle = BorderStyle.FixedSingle };
            gbCust.Controls.AddRange(new Control[] { lP, txtCustPhone, btnFindCust, lblCustStatus, lN, txtCustName, btnResetCust, lA, txtCustAddress });

            // Chọn sp
            var gbItem = new GroupBox { Text = "2. Bỏ giỏ hàng", Location = new Point(10, 200), Size = new Size(420, 185), Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = COLOR_DARK_TEXT, BackColor = Color.White };
            var lSearch = new Label { Text = "🔍 Tìm nhanh:", Location = new Point(15, 35), Size = new Size(100, 25), ForeColor = COLOR_PRIMARY_ROSE };
            txtSearchProduct = new TextBox { Location = new Point(115, 33), Size = new Size(290, 25), BorderStyle = BorderStyle.FixedSingle };
            txtSearchProduct.PlaceholderText = "Gõ tên nến hoặc mùi hương...";
            txtSearchProduct.TextChanged += TxtSearchProduct_TextChanged;

            var lS = new Label { Text = "Mặt hàng:", Location = new Point(15, 75), Size = new Size(100, 25) };
            cbProducts = new ComboBox { Location = new Point(115, 73), Size = new Size(290, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            
            var lQ = new Label { Text = "Số lượng:", Location = new Point(15, 115), Size = new Size(100, 25) };
            txtSaleQty = new TextBox { Location = new Point(115, 113), Size = new Size(70, 25), Text = "1", BorderStyle = BorderStyle.FixedSingle, TextAlign = HorizontalAlignment.Center };
            
            btnAddCart = new Button { Text = "➕ Thêm Giỏ", Location = new Point(195, 111), Size = new Size(210, 28), BackColor = COLOR_PRIMARY_ROSE, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnRemoveCart = new Button { Text = "❌ Xóa Món", Location = new Point(15, 150), Size = new Size(190, 28), BackColor = Color.Peru, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnClearCart = new Button { Text = "🗑️ Hủy Toàn Đơn", Location = new Point(215, 150), Size = new Size(190, 28), BackColor = Color.DarkGray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            gbItem.Controls.AddRange(new Control[] { lSearch, txtSearchProduct, lS, cbProducts, lQ, txtSaleQty, btnAddCart, btnRemoveCart, btnClearCart });

            // Thanh toán
            lblTotalCart = new Label { Text = "TỔNG: 0 đ", Location = new Point(10, 400), Size = new Size(420, 50), Font = new Font("Segoe UI", 20, FontStyle.Bold), ForeColor = COLOR_PRIMARY_ROSE, TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            btnCheckout = new Button { Text = "💳 XÁC NHẬN THANH TOÁN QUẦY", Location = new Point(10, 465), Size = new Size(420, 60), BackColor = Color.SeaGreen, ForeColor = Color.White, Font = new Font("Segoe UI", 12, FontStyle.Bold), FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };

            // Giỏ hàng
            var lblCartTitle = new Label { Text = "CHI TIẾT ĐƠN HÀNG ĐANG LẬP", Location = new Point(445, 15), Size = new Size(300, 25), Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = COLOR_DARK_TEXT };
            dgvCart = new DataGridView { Location = new Point(445, 45), Size = new Size(585, 480), AllowUserToAddRows = false, RowHeadersVisible = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            StyleGrid(dgvCart);

            InitializeCartTable();
            LoadSalesProductsToCache();

            // Gắn sự kiện
            btnFindCust.Click += BtnFindCust_Click; 
            btnResetCust.Click += (s, e) => ClearCustomerArea();
            btnAddCart.Click += BtnAddCart_Click; 
            btnRemoveCart.Click += BtnRemoveCart_Click;
            btnClearCart.Click += (s, e) => ResetFullOrder();
            btnCheckout.Click += BtnCheckout_Click;

            pnl.Controls.AddRange(new Control[] { gbCust, gbItem, lblTotalCart, btnCheckout, lblCartTitle, dgvCart });
            return pnl;
        }

        private void InitializeCartTable()
        {
            // 1. Tự khai báo cột cho Grid
            dgvCart.AutoGenerateColumns = false;
            dgvCart.Columns.Clear();
            
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MaSP", Name = "MaSP", HeaderText = "Mã" });
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TenSP", Name = "TenSP", HeaderText = "Mặt Hàng" });
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SoLuong", Name = "SoLuong", HeaderText = "SL" });
            
            var colDonGia = new DataGridViewTextBoxColumn { DataPropertyName = "DonGia", Name = "DonGia", HeaderText = "Đơn Giá" };
            colDonGia.DefaultCellStyle.Format = "#,##0";
            dgvCart.Columns.Add(colDonGia);
            
            var colThanhTien = new DataGridViewTextBoxColumn { DataPropertyName = "ThanhTien", Name = "ThanhTien", HeaderText = "Thành Tiền" };
            colThanhTien.DefaultCellStyle.Format = "#,##0";
            dgvCart.Columns.Add(colThanhTien);

            // 2. Tạo DataTable lưu dữ liệu 
            dtCartBind = new DataTable();
            dtCartBind.Columns.Add("MaSP", typeof(int)); 
            dtCartBind.Columns.Add("TenSP", typeof(string));
            dtCartBind.Columns.Add("SoLuong", typeof(int)); 
            dtCartBind.Columns.Add("DonGia", typeof(decimal));
            dtCartBind.Columns.Add("ThanhTien", typeof(decimal));
            
            // 3. Bind dữ liệu an toàn
            dgvCart.DataSource = dtCartBind;
        }

        private void LoadSalesProductsToCache()
        {
            var res = sanPhamBUS.GetAllProducts();
            if (res.IsSuccess && res.Data != null) { allProductsCache = res.Data; BindComboBox(allProductsCache); }
        }

        private void BindComboBox(List<SanPhamDTO> list)
        {
            cbProducts.DataSource = null; cbProducts.DataSource = list;
            cbProducts.DisplayMember = "TenSP"; cbProducts.ValueMember = "MaSP";
        }

        private void TxtSearchProduct_TextChanged(object? sender, EventArgs e)
        {
            string keyword = txtSearchProduct.Text.Trim().ToLower();
            var filteredList = allProductsCache.Where(p => p.TenSP.ToLower().Contains(keyword) || p.MuiHuong.ToLower().Contains(keyword)).ToList();
            BindComboBox(filteredList);
        }

        private void BtnFindCust_Click(object? sender, EventArgs e)
        {
            var res = khachHangBUS.FindCustomerByPhone(txtCustPhone.Text.Trim());
            if (res.IsSuccess && res.Data != null) {
                activeCustomer = res.Data; txtCustName.Text = res.Data.TenKH; txtCustAddress.Text = res.Data.DiaChi;
                txtCustName.ReadOnly = true; txtCustAddress.ReadOnly = true; txtCustName.BackColor = Color.LightGray; txtCustAddress.BackColor = Color.LightGray;
                lblCustStatus.Text = $"✨ Đã gán: {res.Data.TenKH} (Mã KH: {res.Data.MaKH}) ✨"; lblCustStatus.ForeColor = Color.Green;
            } else {
                activeCustomer = null; txtCustName.Clear(); txtCustAddress.Clear();
                txtCustName.ReadOnly = false; txtCustAddress.ReadOnly = false; txtCustName.BackColor = Color.White; txtCustAddress.BackColor = Color.White;
                lblCustStatus.Text = "👤 Khách mới (Sẽ tự tạo thành viên)"; lblCustStatus.ForeColor = Color.DarkOrange;
            }
        }

        private void ClearCustomerArea()
        {
            activeCustomer = null; txtCustPhone.Clear(); txtCustName.Clear(); txtCustAddress.Clear();
            txtCustName.ReadOnly = false; txtCustAddress.ReadOnly = false; txtCustName.BackColor = Color.White; txtCustAddress.BackColor = Color.White;
            lblCustStatus.Text = "📢 Đang đặt ở chế độ: KHÁCH VÃNG LAI"; lblCustStatus.ForeColor = Color.DarkOrange;
        }

        private void BtnAddCart_Click(object? sender, EventArgs e)
        {
            if (cbProducts.SelectedItem == null) { MessageBox.Show("Không có mặt hàng!"); return; }
            if (!int.TryParse(txtSaleQty.Text.Trim(), out int qty) || qty <= 9) { MessageBox.Show("Số lượng phải > 10!"); return; }

            SanPhamDTO selectedSp = (SanPhamDTO)cbProducts.SelectedItem;
            DataRow[] existing = dtCartBind.Select($"MaSP = {selectedSp.MaSP}");
            if (existing.Length > 0) {
                int newQty = (int)existing[0]["SoLuong"] + qty;
                existing[0]["SoLuong"] = newQty; existing[0]["ThanhTien"] = newQty * selectedSp.Gia;
            } else {
                dtCartBind.Rows.Add(selectedSp.MaSP, selectedSp.TenSP, qty, selectedSp.Gia, qty * selectedSp.Gia);
            }
            UpdateCartTotal(); txtSearchProduct.Clear();
        }

        private void BtnRemoveCart_Click(object? sender, EventArgs e)
        {
            if (dgvCart.CurrentRow != null && dgvCart.CurrentRow.Index >= 0) {
                int maSP = (int)dgvCart.CurrentRow.Cells["MaSP"].Value;
                foreach (var row in dtCartBind.Select($"MaSP = {maSP}")) dtCartBind.Rows.Remove(row);
                UpdateCartTotal();
            } else MessageBox.Show("Hãy chọn 1 mặt hàng trên lưới để xóa!");
        }

        private void UpdateCartTotal()
        {
            decimal total = 0; currentCart.Clear();
            foreach (DataRow row in dtCartBind.Rows) {
                total += (decimal)row["ThanhTien"];
                currentCart.Add(new ChiTietHoaDonDTO { MaSP = (int)row["MaSP"], SoLuong = (int)row["SoLuong"], DonGia = (decimal)row["DonGia"] });
            }
            lblTotalCart.Text = $"TỔNG: {total:#,##0} đ";
        }

        private void ResetFullOrder()
        {
            dtCartBind.Rows.Clear(); UpdateCartTotal(); ClearCustomerArea(); txtSearchProduct.Clear(); txtSaleQty.Text = "1";
        }

        private void BtnCheckout_Click(object? sender, EventArgs e)
        {
            if (currentCart.Count == 0) { MessageBox.Show("Giỏ hàng đang trống!"); return; }

            int? finalMaKH = null;
            if (activeCustomer != null) finalMaKH = activeCustomer.MaKH;
            else if (!string.IsNullOrWhiteSpace(txtCustName.Text)) {
                var resKh = khachHangBUS.AddCustomer(new KhachHangDTO { TenKH = txtCustName.Text.Trim(), SDT = txtCustPhone.Text.Trim(), DiaChi = txtCustAddress.Text.Trim() });
                if (resKh.IsSuccess) {
                    var khSaved = khachHangBUS.FindCustomerByPhone(txtCustPhone.Text.Trim());
                    if (khSaved.IsSuccess && khSaved.Data != null) finalMaKH = khSaved.Data.MaKH;
                }
            }

            var res = hoaDonBUS.ProcessCheckout(new HoaDonDTO { MaKH = finalMaKH, MaND = currentStaff.MaND }, currentCart);
            if (res.IsSuccess) {
                MessageBox.Show(res.Message, "Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ResetFullOrder(); LoadSalesProductsToCache();
            } else MessageBox.Show(res.Message, "Lỗi Quầy", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        // ==================== Nhật ký ca làm ====================
        private Panel CreateHistoryPanel()
        {
            var pnl = new Panel { BackColor = Color.White };
            var lbl = new Label { Text = "📑 DANH SÁCH HÓA ĐƠN XUẤT TRONG CA LÀM VIỆC CỦA BẠN", Location = new Point(20, 15), Size = new Size(600, 25), Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = COLOR_DARK_TEXT };
            dgvMyHistory = new DataGridView { Location = new Point(20, 55), Size = new Size(1010, 475), AllowUserToAddRows = false, ReadOnly = true, RowHeadersVisible = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            StyleGrid(dgvMyHistory);
            
            pnl.Controls.AddRange(new Control[] { lbl, dgvMyHistory });
            return pnl;
        }

        private void LoadPersonalSalesHistory()
        {
            var res = hoaDonBUS.GetSalesHistory(currentStaff.MaND, currentStaff.VaiTro);
            if (res.IsSuccess && res.Data != null) {
                
                dgvMyHistory.AutoGenerateColumns = false;
                dgvMyHistory.Columns.Clear();
                dgvMyHistory.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MaHD", HeaderText = "Mã Hóa Đơn" });
                dgvMyHistory.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "NgayLap", HeaderText = "Thời Gian Lập" });
                dgvMyHistory.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TenKhachHang", HeaderText = "Khách Hàng" });
                
                var colTien = new DataGridViewTextBoxColumn { DataPropertyName = "TongTien", HeaderText = "Doanh Số (VND)" };
                colTien.DefaultCellStyle.Format = "#,##0";
                dgvMyHistory.Columns.Add(colTien);

                dgvMyHistory.DataSource = res.Data;
            } else {
                MessageBox.Show($"[Lỗi Tải Lịch Sử]: {res.Message}", "Lỗi DB", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}