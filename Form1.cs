using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageFilterApp
{
    public partial class Form1 : Form
    {
        
        private Panel? panelTop;
        private Button? btnOpenImage;

        
        private TableLayoutPanel? tableMiddle;
        private Panel? panelLeft;
        private Panel? panelRight;
        private PictureBox? pictureLeft;
        private PictureBox? pictureRight;
        private Button? btnLeftStartStop;
        private Button? btnRightNextFilter;
        private Label? lblLeftStatus;
        private Label? lblRightStatus;
        private Label? lblLeftTitle;
        private Label? lblRightTitle;

        
        private Panel? panelBottom;
        private FlowLayoutPanel? flowBottom;
        private Button? btnSaveLeft;
        private Button? btnSaveRight;
        private Button? btnSaveAll;
        private Label? lblGlobalStatus;

        
        private Bitmap? originalImage;
        private string? originalImagePath;

        
        private Bitmap? currentLeftImage;
        private Bitmap? currentRightImage;

        
        private int leftFilterIndex = 0;
        private int rightFilterIndex = 0;

       
        private System.Windows.Forms.Timer? timerLeft;

        
        private bool isProcessingLeft = false;

        
        private List<Func<Bitmap, Bitmap>>? filters;

        
        private List<string>? filterNames;

        
        private const int FILTER_INTERVAL_MS = 1000;

        
        private readonly Color colorMainBg = Color.FromArgb(0x1E, 0x1E, 0x1E);        // #1E1E1E
        private readonly Color colorPanelBg = Color.FromArgb(0x25, 0x25, 0x26);        // #252526
        private readonly Color colorPanelBgAlt = Color.FromArgb(0x2D, 0x2D, 0x30);     // #2D2D30
        private readonly Color colorBorder = Color.FromArgb(0x3E, 0x3E, 0x42);        // #3E3E42
        private readonly Color colorAccent = Color.FromArgb(0x00, 0x7A, 0xCC);          // #007ACC (blue)
        private readonly Color colorText = Color.FromArgb(0xE0, 0xE0, 0xE0);           // #E0E0E0
        private readonly Color colorTextSecondary = Color.FromArgb(0x9E, 0x9E, 0x9E); // #9E9E9E
        private readonly Color colorPictureBg = Color.FromArgb(0x20, 0x20, 0x20);      // #202020

        public Form1()
        {
            InitializeComponent();
            InitializeFilters();
            InitializeTimer();
        }

        
        private void InitializeFilters()
        {
            filters = new List<Func<Bitmap, Bitmap>>
            {
                ApplyGrayscale,
                ApplySepia,
                ApplyInvert,
                ApplyBlur,
                ApplySharpen,
                ApplyBrightness,
                ApplyContrast,
                ApplyEdgeDetection
            };

            filterNames = new List<string>
            {
                "Grayscale",
                "Sepia",
                "Invert",
                "Blur",
                "Sharpen",
                "Brightness",
                "Contrast",
                "Edge Detection"
            };
        }

       
        private void InitializeTimer()
        {
            timerLeft = new System.Windows.Forms.Timer();
            timerLeft.Interval = FILTER_INTERVAL_MS;
            timerLeft.Tick += TimerLeft_Tick;
        }

        
        private void InitializeComponent()
        {
            this.SuspendLayout();

            
            this.Text = "Dual Filter Image Player";
            this.Size = new Size(1200, 700);
            this.MinimumSize = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = colorMainBg;
            this.ForeColor = colorText;
            this.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);

            
            panelTop = new Panel();
            panelTop.Dock = DockStyle.Top;
            panelTop.Height = 80;
            panelTop.BackColor = colorPanelBg;
            panelTop.Padding = new Padding(20, 20, 20, 20);
            this.Controls.Add(panelTop);

            
            btnOpenImage = new Button();
            btnOpenImage.Text = "Open Image from Device";
            btnOpenImage.Size = new Size(400, 45);
            btnOpenImage.Anchor = AnchorStyles.None;
            btnOpenImage.Location = new Point((panelTop.Width - btnOpenImage.Width) / 2, (panelTop.Height - btnOpenImage.Height) / 2);
            btnOpenImage.BackColor = colorAccent;
            btnOpenImage.ForeColor = Color.White;
            btnOpenImage.FlatStyle = FlatStyle.Flat;
            btnOpenImage.FlatAppearance.BorderSize = 0;
            btnOpenImage.FlatAppearance.MouseOverBackColor = Color.FromArgb(0x00, 0x6A, 0xB8);
            btnOpenImage.FlatAppearance.MouseDownBackColor = Color.FromArgb(0x00, 0x5A, 0xA8);
            btnOpenImage.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnOpenImage.Cursor = Cursors.Hand;
            btnOpenImage.Click += BtnOpenImage_Click;
            panelTop.Controls.Add(btnOpenImage);

           
            panelTop.Resize += (s, e) =>
            {
                if (btnOpenImage != null)
                {
                    btnOpenImage.Left = (panelTop.Width - btnOpenImage.Width) / 2;
                    btnOpenImage.Top = (panelTop.Height - btnOpenImage.Height) / 2;
                }
            };

            
            tableMiddle = new TableLayoutPanel();
            tableMiddle.Dock = DockStyle.Fill;
            tableMiddle.ColumnCount = 2;
            tableMiddle.RowCount = 1;
            tableMiddle.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableMiddle.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableMiddle.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableMiddle.Padding = new Padding(10, 10, 10, 10);
            tableMiddle.BackColor = colorMainBg;
            this.Controls.Add(tableMiddle);

            
            panelLeft = new Panel();
            panelLeft.Dock = DockStyle.Fill;
            panelLeft.BackColor = colorPanelBg;
            panelLeft.Padding = new Padding(20);
            panelLeft.Margin = new Padding(5);
            tableMiddle.Controls.Add(panelLeft, 0, 0);

            
            lblLeftTitle = new Label();
            lblLeftTitle.Text = "Automatic Filters (Left)";
            lblLeftTitle.Location = new Point(20, 20);
            lblLeftTitle.AutoSize = false;
            lblLeftTitle.Size = new Size(panelLeft.Width - 40, 28);
            lblLeftTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblLeftTitle.ForeColor = colorText;
            lblLeftTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelLeft.Controls.Add(lblLeftTitle);

            
            pictureLeft = new PictureBox();
            pictureLeft.Location = new Point(20, 55);
            pictureLeft.Size = new Size(panelLeft.Width - 40, panelLeft.Height - 180);
            pictureLeft.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureLeft.SizeMode = PictureBoxSizeMode.Zoom;
            pictureLeft.BorderStyle = BorderStyle.FixedSingle;
            pictureLeft.BackColor = colorPictureBg;
            panelLeft.Controls.Add(pictureLeft);

            
            btnLeftStartStop = new Button();
            btnLeftStartStop.Text = "Start Filters";
            btnLeftStartStop.Size = new Size(220, 40);
            btnLeftStartStop.Location = new Point((panelLeft.Width - btnLeftStartStop.Width) / 2, panelLeft.Height - 90);
            btnLeftStartStop.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnLeftStartStop.BackColor = colorPanelBgAlt;
            btnLeftStartStop.ForeColor = colorText;
            btnLeftStartStop.FlatStyle = FlatStyle.Flat;
            btnLeftStartStop.FlatAppearance.BorderSize = 1;
            btnLeftStartStop.FlatAppearance.BorderColor = colorBorder;
            btnLeftStartStop.FlatAppearance.MouseOverBackColor = Color.FromArgb(0x3E, 0x3E, 0x42);
            btnLeftStartStop.FlatAppearance.MouseDownBackColor = Color.FromArgb(0x45, 0x45, 0x49);
            btnLeftStartStop.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            btnLeftStartStop.Cursor = Cursors.Hand;
            btnLeftStartStop.Click += BtnLeftStartStop_Click;
            panelLeft.Controls.Add(btnLeftStartStop);

            
            lblLeftStatus = new Label();
            lblLeftStatus.Text = "Ready";
            lblLeftStatus.Location = new Point(20, panelLeft.Height - 40);
            lblLeftStatus.Size = new Size(panelLeft.Width - 40, 22);
            lblLeftStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblLeftStatus.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            lblLeftStatus.ForeColor = colorTextSecondary;
            lblLeftStatus.TextAlign = ContentAlignment.MiddleCenter;
            panelLeft.Controls.Add(lblLeftStatus);

            
            panelRight = new Panel();
            panelRight.Dock = DockStyle.Fill;
            panelRight.BackColor = colorPanelBg;
            panelRight.Padding = new Padding(20);
            panelRight.Margin = new Padding(5);
            tableMiddle.Controls.Add(panelRight, 1, 0);

            
            lblRightTitle = new Label();
            lblRightTitle.Text = "Manual Filters (Right)";
            lblRightTitle.Location = new Point(20, 20);
            lblRightTitle.AutoSize = false;
            lblRightTitle.Size = new Size(panelRight.Width - 40, 28);
            lblRightTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblRightTitle.ForeColor = colorText;
            lblRightTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelRight.Controls.Add(lblRightTitle);

            
            pictureRight = new PictureBox();
            pictureRight.Location = new Point(20, 55);
            pictureRight.Size = new Size(panelRight.Width - 40, panelRight.Height - 180);
            pictureRight.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureRight.SizeMode = PictureBoxSizeMode.Zoom;
            pictureRight.BorderStyle = BorderStyle.FixedSingle;
            pictureRight.BackColor = colorPictureBg;
            panelRight.Controls.Add(pictureRight);

            
            btnRightNextFilter = new Button();
            btnRightNextFilter.Text = "Next Filter";
            btnRightNextFilter.Size = new Size(220, 40);
            btnRightNextFilter.Location = new Point((panelRight.Width - btnRightNextFilter.Width) / 2, panelRight.Height - 90);
            btnRightNextFilter.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnRightNextFilter.BackColor = colorPanelBgAlt;
            btnRightNextFilter.ForeColor = colorText;
            btnRightNextFilter.FlatStyle = FlatStyle.Flat;
            btnRightNextFilter.FlatAppearance.BorderSize = 1;
            btnRightNextFilter.FlatAppearance.BorderColor = colorBorder;
            btnRightNextFilter.FlatAppearance.MouseOverBackColor = Color.FromArgb(0x3E, 0x3E, 0x42);
            btnRightNextFilter.FlatAppearance.MouseDownBackColor = Color.FromArgb(0x45, 0x45, 0x49);
            btnRightNextFilter.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            btnRightNextFilter.Cursor = Cursors.Hand;
            btnRightNextFilter.Click += BtnRightNextFilter_Click;
            panelRight.Controls.Add(btnRightNextFilter);

            
            lblRightStatus = new Label();
            lblRightStatus.Text = "Ready";
            lblRightStatus.Location = new Point(20, panelRight.Height - 40);
            lblRightStatus.Size = new Size(panelRight.Width - 40, 22);
            lblRightStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblRightStatus.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            lblRightStatus.ForeColor = colorTextSecondary;
            lblRightStatus.TextAlign = ContentAlignment.MiddleCenter;
            panelRight.Controls.Add(lblRightStatus);

            
            panelBottom = new Panel();
            panelBottom.Dock = DockStyle.Bottom;
            panelBottom.Height = 80;
            panelBottom.BackColor = colorPanelBg;
            panelBottom.Padding = new Padding(20, 20, 20, 20);
            this.Controls.Add(panelBottom);

            
            flowBottom = new FlowLayoutPanel();
            flowBottom.Dock = DockStyle.Fill;
            flowBottom.FlowDirection = FlowDirection.LeftToRight;
            flowBottom.WrapContents = false;
            flowBottom.BackColor = Color.Transparent;
            panelBottom.Controls.Add(flowBottom);

            
            btnSaveLeft = new Button();
            btnSaveLeft.Text = "Save Left Image";
            btnSaveLeft.Size = new Size(180, 38);
            btnSaveLeft.Margin = new Padding(0, 0, 12, 0);
            btnSaveLeft.BackColor = colorPanelBgAlt;
            btnSaveLeft.ForeColor = colorText;
            btnSaveLeft.FlatStyle = FlatStyle.Flat;
            btnSaveLeft.FlatAppearance.BorderSize = 1;
            btnSaveLeft.FlatAppearance.BorderColor = colorBorder;
            btnSaveLeft.FlatAppearance.MouseOverBackColor = Color.FromArgb(0x3E, 0x3E, 0x42);
            btnSaveLeft.FlatAppearance.MouseDownBackColor = Color.FromArgb(0x45, 0x45, 0x49);
            btnSaveLeft.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            btnSaveLeft.Cursor = Cursors.Hand;
            btnSaveLeft.Click += BtnSaveLeft_Click;
            flowBottom.Controls.Add(btnSaveLeft);

            
            btnSaveRight = new Button();
            btnSaveRight.Text = "Save Right Image";
            btnSaveRight.Size = new Size(180, 38);
            btnSaveRight.Margin = new Padding(0, 0, 12, 0);
            btnSaveRight.BackColor = colorPanelBgAlt;
            btnSaveRight.ForeColor = colorText;
            btnSaveRight.FlatStyle = FlatStyle.Flat;
            btnSaveRight.FlatAppearance.BorderSize = 1;
            btnSaveRight.FlatAppearance.BorderColor = colorBorder;
            btnSaveRight.FlatAppearance.MouseOverBackColor = Color.FromArgb(0x3E, 0x3E, 0x42);
            btnSaveRight.FlatAppearance.MouseDownBackColor = Color.FromArgb(0x45, 0x45, 0x49);
            btnSaveRight.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            btnSaveRight.Cursor = Cursors.Hand;
            btnSaveRight.Click += BtnSaveRight_Click;
            flowBottom.Controls.Add(btnSaveRight);

            
            btnSaveAll = new Button();
            btnSaveAll.Text = "Save All";
            btnSaveAll.Size = new Size(180, 38);
            btnSaveAll.Margin = new Padding(0, 0, 20, 0);
            btnSaveAll.BackColor = colorAccent;
            btnSaveAll.ForeColor = Color.White;
            btnSaveAll.FlatStyle = FlatStyle.Flat;
            btnSaveAll.FlatAppearance.BorderSize = 0;
            btnSaveAll.FlatAppearance.MouseOverBackColor = Color.FromArgb(0x00, 0x6A, 0xB8);
            btnSaveAll.FlatAppearance.MouseDownBackColor = Color.FromArgb(0x00, 0x5A, 0xA8);
            btnSaveAll.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnSaveAll.Cursor = Cursors.Hand;
            btnSaveAll.Click += BtnSaveAll_Click;
            flowBottom.Controls.Add(btnSaveAll);

            
            lblGlobalStatus = new Label();
            lblGlobalStatus.Text = "Ready";
            lblGlobalStatus.AutoSize = false;
            lblGlobalStatus.Size = new Size(350, 38);
            lblGlobalStatus.Margin = new Padding(0);
            lblGlobalStatus.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            lblGlobalStatus.ForeColor = colorTextSecondary;
            lblGlobalStatus.TextAlign = ContentAlignment.MiddleRight;
            lblGlobalStatus.Anchor = AnchorStyles.Right;
            flowBottom.Controls.Add(lblGlobalStatus);

            
            if (panelLeft != null)
                panelLeft.Resize += (s, e) => LayoutLeftPanel();
            if (panelRight != null)
                panelRight.Resize += (s, e) => LayoutRightPanel();

            
            LayoutLeftPanel();
            LayoutRightPanel();

            this.ResumeLayout(false);
        }

        
        private void LayoutLeftPanel()
        {
            if (panelLeft == null || pictureLeft == null || btnLeftStartStop == null || lblLeftStatus == null || lblLeftTitle == null)
                return;

            int padding = 20;
            int titleHeight = 28;
            int spaceBetween = 8;
            int bottomAreaHeight = 70; 

            int w = panelLeft.ClientSize.Width;
            int h = panelLeft.ClientSize.Height;

            
            lblLeftTitle.SetBounds(
                padding,
                padding,
                w - padding * 2,
                titleHeight
            );

            
            int pictureTop = padding + titleHeight + spaceBetween;
            int pictureHeight = h - pictureTop - bottomAreaHeight - padding;
            if (pictureHeight < 50) pictureHeight = 50;

            pictureLeft.SetBounds(
                padding,
                pictureTop,
                w - padding * 2,
                pictureHeight
            );

            
            int buttonHeight = 40;
            int buttonWidth = 220;
            int buttonTop = h - padding - bottomAreaHeight + 5;

            btnLeftStartStop.SetBounds(
                (w - buttonWidth) / 2,
                buttonTop,
                buttonWidth,
                buttonHeight
            );

            
            lblLeftStatus.SetBounds(
                padding,
                h - padding - 22,
                w - padding * 2,
                22
            );
        }

        
        private void LayoutRightPanel()
        {
            if (panelRight == null || pictureRight == null || btnRightNextFilter == null || lblRightStatus == null || lblRightTitle == null)
                return;

            int padding = 20;
            int titleHeight = 28;
            int spaceBetween = 8;
            int bottomAreaHeight = 70;

            int w = panelRight.ClientSize.Width;
            int h = panelRight.ClientSize.Height;

            
            lblRightTitle.SetBounds(
                padding,
                padding,
                w - padding * 2,
                titleHeight
            );

           
            int pictureTop = padding + titleHeight + spaceBetween;
            int pictureHeight = h - pictureTop - bottomAreaHeight - padding;
            if (pictureHeight < 50) pictureHeight = 50;

            pictureRight.SetBounds(
                padding,
                pictureTop,
                w - padding * 2,
                pictureHeight
            );

            
            int buttonHeight = 40;
            int buttonWidth = 220;
            int buttonTop = h - padding - bottomAreaHeight + 5;

            btnRightNextFilter.SetBounds(
                (w - buttonWidth) / 2,
                buttonTop,
                buttonWidth,
                buttonHeight
            );

            
            lblRightStatus.SetBounds(
                padding,
                h - padding - 22,
                w - padding * 2,
                22
            );
        }

        #region Top Section - Open Image

       
        private void BtnOpenImage_Click(object? sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image files (*.jpg;*.jpeg;*.png;*.bmp;*.gif)|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        
                        originalImage?.Dispose();
                        currentLeftImage?.Dispose();
                        currentRightImage?.Dispose();
                        if (pictureLeft != null && pictureLeft.Image != null && pictureLeft.Image != originalImage && pictureLeft.Image != currentLeftImage)
                        {
                            pictureLeft.Image.Dispose();
                        }
                        if (pictureRight != null && pictureRight.Image != null && pictureRight.Image != originalImage && pictureRight.Image != currentRightImage)
                        {
                            pictureRight.Image.Dispose();
                        }

                        
                        originalImage = new Bitmap(openFileDialog.FileName);
                        originalImagePath = openFileDialog.FileName;

                        
                        if (pictureLeft != null)
                            pictureLeft.Image = new Bitmap(originalImage);
                        if (pictureRight != null)
                            pictureRight.Image = new Bitmap(originalImage);

                        
                        currentLeftImage = new Bitmap(originalImage);
                        currentRightImage = new Bitmap(originalImage);

                        
                        leftFilterIndex = 0;
                        rightFilterIndex = 0;

                        
                        if (timerLeft != null && timerLeft.Enabled)
                        {
                            timerLeft.Stop();
                            if (btnLeftStartStop != null)
                                btnLeftStartStop.Text = "Start Filters";
                        }

                        
                        UpdateLeftStatus("Original");
                        UpdateRightStatus("Original");
                        UpdateGlobalStatus($"Opened image: {Path.GetFileName(originalImagePath)}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        UpdateGlobalStatus("Error loading image");
                    }
                }
            }
        }

        #endregion

        #region Left Side - Start/Stop Filters

        
        private void BtnLeftStartStop_Click(object? sender, EventArgs e)
        {
            if (originalImage == null)
            {
                MessageBox.Show("Please open an image first.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateGlobalStatus("Please open an image first");
                return;
            }

            if (timerLeft != null && timerLeft.Enabled)
            {
                
                timerLeft.Stop();
                if (btnLeftStartStop != null)
                    btnLeftStartStop.Text = "Start Filters";
                if (filterNames != null && leftFilterIndex < filterNames.Count)
                    UpdateLeftStatus($"Stopped: {filterNames[leftFilterIndex]}");
                else
                    UpdateLeftStatus("Stopped");
                UpdateGlobalStatus("Filter cycling stopped");
            }
            else if (timerLeft != null)
            {
                
                timerLeft.Start();
                if (btnLeftStartStop != null)
                    btnLeftStartStop.Text = "Stop";
                UpdateLeftStatus("Running...");
                UpdateGlobalStatus("Filter cycling started");
            }
        }

        
        private async void TimerLeft_Tick(object? sender, EventArgs e)
        {
            
            if (isProcessingLeft || originalImage == null)
                return;

            await ApplyNextFilterLeft();
        }

        
        private async Task ApplyNextFilterLeft()
        {
            if (originalImage == null || filters == null || filterNames == null) return;

            isProcessingLeft = true;
            if (lblLeftStatus != null && filterNames != null)
            {
                if (leftFilterIndex < filterNames.Count)
                    UpdateLeftStatus($"Processing: {filterNames[leftFilterIndex]}...");
                else
                    UpdateLeftStatus("Processing...");
            }
            Application.DoEvents(); 

            try
            {
                
                if (filters == null || filterNames == null) return;
                Func<Bitmap, Bitmap> filter = filters[leftFilterIndex];
                string filterName = filterNames[leftFilterIndex];

                
                Bitmap filteredImage = await Task.Run(() => filter(new Bitmap(originalImage)));

                
                if (pictureLeft != null)
                {
                    if (pictureLeft.InvokeRequired)
                    {
                        pictureLeft.Invoke(new Action(() =>
                        {
                            if (currentLeftImage != null && currentLeftImage != originalImage)
                            {
                                currentLeftImage.Dispose();
                            }
                            currentLeftImage = filteredImage;
                            if (pictureLeft.Image != null && pictureLeft.Image != originalImage && pictureLeft.Image != currentLeftImage)
                            {
                                pictureLeft.Image.Dispose();
                            }
                            pictureLeft.Image = filteredImage;
                        }));
                    }
                    else
                    {
                        if (currentLeftImage != null && currentLeftImage != originalImage)
                        {
                            currentLeftImage.Dispose();
                        }
                        currentLeftImage = filteredImage;
                        if (pictureLeft.Image != null && pictureLeft.Image != originalImage && pictureLeft.Image != currentLeftImage)
                        {
                            pictureLeft.Image.Dispose();
                        }
                        pictureLeft.Image = filteredImage;
                    }
                }

                
                UpdateLeftStatus($"Running: {filterName}");
                UpdateGlobalStatus($"Left: {filterName}");

                
                leftFilterIndex = (leftFilterIndex + 1) % filters.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying filter: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateLeftStatus("Error");
                UpdateGlobalStatus("Error applying filter");
            }
            finally
            {
                isProcessingLeft = false;
            }
        }

        #endregion

        #region Right Side - Next Filter

       
        private async void BtnRightNextFilter_Click(object? sender, EventArgs e)
        {
            if (originalImage == null)
            {
                MessageBox.Show("Please open an image first.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateGlobalStatus("Please open an image first");
                return;
            }

            await ApplyNextFilterRight();
        }

        
        private async Task ApplyNextFilterRight()
        {
            if (originalImage == null || filters == null || filterNames == null) return;

            if (lblRightStatus != null && filterNames != null)
            {
                if (rightFilterIndex < filterNames.Count)
                    UpdateRightStatus($"Processing: {filterNames[rightFilterIndex]}...");
                else
                    UpdateRightStatus("Processing...");
            }
            Application.DoEvents(); 

            try
            {
                
                if (filters == null || filterNames == null) return;
                Func<Bitmap, Bitmap> filter = filters[rightFilterIndex];
                string filterName = filterNames[rightFilterIndex];

                
                Bitmap filteredImage = await Task.Run(() => filter(new Bitmap(originalImage)));

                
                if (pictureRight != null)
                {
                    if (pictureRight.InvokeRequired)
                    {
                        pictureRight.Invoke(new Action(() =>
                        {
                            if (currentRightImage != null && currentRightImage != originalImage)
                            {
                                currentRightImage.Dispose();
                            }
                            currentRightImage = filteredImage;
                            if (pictureRight.Image != null && pictureRight.Image != originalImage && pictureRight.Image != currentRightImage)
                            {
                                pictureRight.Image.Dispose();
                            }
                            pictureRight.Image = filteredImage;
                        }));
                    }
                    else
                    {
                        if (currentRightImage != null && currentRightImage != originalImage)
                        {
                            currentRightImage.Dispose();
                        }
                        currentRightImage = filteredImage;
                        if (pictureRight.Image != null && pictureRight.Image != originalImage && pictureRight.Image != currentRightImage)
                        {
                            pictureRight.Image.Dispose();
                        }
                        pictureRight.Image = filteredImage;
                    }
                }

                
                UpdateRightStatus($"Right: {filterName}");
                UpdateGlobalStatus($"Right: {filterName}");

                
                rightFilterIndex = (rightFilterIndex + 1) % filters.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying filter: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateRightStatus("Error");
                UpdateGlobalStatus("Error applying filter");
            }
            finally
            {
            }
        }

        #endregion

        #region Bottom Section - Save Images

        
        private void BtnSaveLeft_Click(object? sender, EventArgs e)
        {
            if (currentLeftImage == null)
            {
                MessageBox.Show("No image to save.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateGlobalStatus("No left image to save");
                return;
            }

            SaveImage(currentLeftImage, "left");
        }

        
        private void BtnSaveRight_Click(object? sender, EventArgs e)
        {
            if (currentRightImage == null)
            {
                MessageBox.Show("No image to save.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateGlobalStatus("No right image to save");
                return;
            }

            SaveImage(currentRightImage, "right");
        }

        
        private void BtnSaveAll_Click(object? sender, EventArgs e)
        {
            if (currentLeftImage == null && currentRightImage == null)
            {
                MessageBox.Show("No images to save.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateGlobalStatus("No images to save");
                return;
            }

            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select a folder to save the processed images";
                folderDialog.ShowNewFolderButton = true;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string folderPath = folderDialog.SelectedPath;
                        string baseName = "image";

                        
                        if (!string.IsNullOrEmpty(originalImagePath))
                        {
                            baseName = Path.GetFileNameWithoutExtension(originalImagePath);
                        }

                        int savedCount = 0;

                        
                        if (currentLeftImage != null)
                        {
                            string leftPath = Path.Combine(folderPath, $"{baseName}_left_filtered.png");
                            currentLeftImage.Save(leftPath, ImageFormat.Png);
                            savedCount++;
                        }

                    
                        if (currentRightImage != null)
                        {
                            string rightPath = Path.Combine(folderPath, $"{baseName}_right_filtered.png");
                            currentRightImage.Save(rightPath, ImageFormat.Png);
                            savedCount++;
                        }

                        string message = savedCount == 2
                            ? $"All images saved successfully to:\n{folderPath}"
                            : $"{savedCount} image(s) saved successfully to:\n{folderPath}";

                        MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        UpdateGlobalStatus($"All images saved to: {folderPath}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving images: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        UpdateGlobalStatus("Error saving images");
                    }
                }
            }
        }

       
        private void SaveImage(Bitmap image, string side)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp|GIF Image|*.gif";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;

                
                if (!string.IsNullOrEmpty(originalImagePath))
                {
                    string baseName = Path.GetFileNameWithoutExtension(originalImagePath);
                    saveFileDialog.FileName = $"{baseName}_{side}_filtered";
                }
                else
                {
                    saveFileDialog.FileName = $"{side}_filtered";
                }

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        ImageFormat format = ImageFormat.Png;
                        string ext = Path.GetExtension(saveFileDialog.FileName).ToLower();
                        if (ext == ".jpg" || ext == ".jpeg") format = ImageFormat.Jpeg;
                        else if (ext == ".bmp") format = ImageFormat.Bmp;
                        else if (ext == ".gif") format = ImageFormat.Gif;

                        image.Save(saveFileDialog.FileName, format);
                        MessageBox.Show("Image saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        UpdateGlobalStatus($"Saved {side} image: {Path.GetFileName(saveFileDialog.FileName)}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        UpdateGlobalStatus("Error saving image");
                    }
                }
            }
        }

        
        private void UpdateLeftStatus(string message)
        {
            if (lblLeftStatus != null)
            {
                if (lblLeftStatus.InvokeRequired)
                {
                    lblLeftStatus.Invoke(new Action(() => lblLeftStatus.Text = message));
                }
                else
                {
                    lblLeftStatus.Text = message;
                }
            }
        }

        
        private void UpdateRightStatus(string message)
        {
            if (lblRightStatus != null)
            {
                if (lblRightStatus.InvokeRequired)
                {
                    lblRightStatus.Invoke(new Action(() => lblRightStatus.Text = message));
                }
                else
                {
                    lblRightStatus.Text = message;
                }
            }
        }

        
        private void UpdateGlobalStatus(string message)
        {
            if (lblGlobalStatus != null)
            {
                if (lblGlobalStatus.InvokeRequired)
                {
                    lblGlobalStatus.Invoke(new Action(() => lblGlobalStatus.Text = message));
                }
                else
                {
                    lblGlobalStatus.Text = message;
                }
            }
        }

        #endregion

        #region Image Filter Implementations

        
        private Bitmap ApplyGrayscale(Bitmap original)
        {
            Bitmap result = new Bitmap(original.Width, original.Height);

            for (int x = 0; x < original.Width; x++)
            {
                for (int y = 0; y < original.Height; y++)
                {
                    Color pixel = original.GetPixel(x, y);
                    int gray = (int)(pixel.R * 0.299 + pixel.G * 0.587 + pixel.B * 0.114);
                    result.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
                }
            }

            return result;
        }

        
        private Bitmap ApplySepia(Bitmap original)
        {
            Bitmap result = new Bitmap(original.Width, original.Height);

            for (int x = 0; x < original.Width; x++)
            {
                for (int y = 0; y < original.Height; y++)
                {
                    Color pixel = original.GetPixel(x, y);
                    int tr = (int)(pixel.R * 0.393 + pixel.G * 0.769 + pixel.B * 0.189);
                    int tg = (int)(pixel.R * 0.349 + pixel.G * 0.686 + pixel.B * 0.168);
                    int tb = (int)(pixel.R * 0.272 + pixel.G * 0.534 + pixel.B * 0.131);

                    tr = Math.Min(255, tr);
                    tg = Math.Min(255, tg);
                    tb = Math.Min(255, tb);

                    result.SetPixel(x, y, Color.FromArgb(tr, tg, tb));
                }
            }

            return result;
        }

        
        private Bitmap ApplyInvert(Bitmap original)
        {
            Bitmap result = new Bitmap(original.Width, original.Height);

            for (int x = 0; x < original.Width; x++)
            {
                for (int y = 0; y < original.Height; y++)
                {
                    Color pixel = original.GetPixel(x, y);
                    result.SetPixel(x, y, Color.FromArgb(255 - pixel.R, 255 - pixel.G, 255 - pixel.B));
                }
            }

            return result;
        }

        
        private Bitmap ApplyBlur(Bitmap original)
        {
            Bitmap result = new Bitmap(original.Width, original.Height);
            int blurSize = 5;

            for (int x = 0; x < original.Width; x++)
            {
                for (int y = 0; y < original.Height; y++)
                {
                    int r = 0, g = 0, b = 0;
                    int count = 0;

                    for (int dx = -blurSize / 2; dx <= blurSize / 2; dx++)
                    {
                        for (int dy = -blurSize / 2; dy <= blurSize / 2; dy++)
                        {
                            int nx = x + dx;
                            int ny = y + dy;

                            if (nx >= 0 && nx < original.Width && ny >= 0 && ny < original.Height)
                            {
                                Color pixel = original.GetPixel(nx, ny);
                                r += pixel.R;
                                g += pixel.G;
                                b += pixel.B;
                                count++;
                            }
                        }
                    }

                    if (count > 0)
                    {
                        r /= count;
                        g /= count;
                        b /= count;
                    }

                    result.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }

            return result;
        }

        
        private Bitmap ApplySharpen(Bitmap original)
        {
            Bitmap result = new Bitmap(original.Width, original.Height);
            int[,] sharpenKernel = { { 0, -1, 0 }, { -1, 5, -1 }, { 0, -1, 0 } };

            for (int x = 1; x < original.Width - 1; x++)
            {
                for (int y = 1; y < original.Height - 1; y++)
                {
                    int r = 0, g = 0, b = 0;

                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            Color pixel = original.GetPixel(x + i, y + j);
                            int kernelValue = sharpenKernel[i + 1, j + 1];
                            r += pixel.R * kernelValue;
                            g += pixel.G * kernelValue;
                            b += pixel.B * kernelValue;
                        }
                    }

                    r = Math.Max(0, Math.Min(255, r));
                    g = Math.Max(0, Math.Min(255, g));
                    b = Math.Max(0, Math.Min(255, b));

                    result.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }

            return result;
        }

        
        private Bitmap ApplyBrightness(Bitmap original)
        {
            Bitmap result = new Bitmap(original.Width, original.Height);
            int brightness = 50;

            for (int x = 0; x < original.Width; x++)
            {
                for (int y = 0; y < original.Height; y++)
                {
                    Color pixel = original.GetPixel(x, y);
                    int r = Math.Min(255, pixel.R + brightness);
                    int g = Math.Min(255, pixel.G + brightness);
                    int b = Math.Min(255, pixel.B + brightness);

                    result.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }

            return result;
        }

       
        private Bitmap ApplyContrast(Bitmap original)
        {
            Bitmap result = new Bitmap(original.Width, original.Height);
            double contrast = 1.5;

            for (int x = 0; x < original.Width; x++)
            {
                for (int y = 0; y < original.Height; y++)
                {
                    Color pixel = original.GetPixel(x, y);
                    int r = (int)Math.Min(255, Math.Max(0, (pixel.R - 128) * contrast + 128));
                    int g = (int)Math.Min(255, Math.Max(0, (pixel.G - 128) * contrast + 128));
                    int b = (int)Math.Min(255, Math.Max(0, (pixel.B - 128) * contrast + 128));

                    result.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }

            return result;
        }

        
        private Bitmap ApplyEdgeDetection(Bitmap original)
        {
            Bitmap result = new Bitmap(original.Width, original.Height);
            Bitmap grayscale = ApplyGrayscale(original);

            int[,] sobelX = { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            int[,] sobelY = { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };

            for (int x = 1; x < grayscale.Width - 1; x++)
            {
                for (int y = 1; y < grayscale.Height - 1; y++)
                {
                    int gx = 0, gy = 0;

                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            Color pixel = grayscale.GetPixel(x + i, y + j);
                            int gray = pixel.R;
                            gx += gray * sobelX[i + 1, j + 1];
                            gy += gray * sobelY[i + 1, j + 1];
                        }
                    }

                    int magnitude = (int)Math.Sqrt(gx * gx + gy * gy);
                    magnitude = Math.Min(255, magnitude);
                    result.SetPixel(x, y, Color.FromArgb(magnitude, magnitude, magnitude));
                }
            }

            grayscale.Dispose();
            return result;
        }

        #endregion

        
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);

            
            timerLeft?.Stop();
            timerLeft?.Dispose();

            
            originalImage?.Dispose();
            currentLeftImage?.Dispose();
            currentRightImage?.Dispose();
            if (pictureLeft != null && pictureLeft.Image != null && pictureLeft.Image != originalImage && pictureLeft.Image != currentLeftImage)
            {
                pictureLeft.Image.Dispose();
            }
            if (pictureRight != null && pictureRight.Image != null && pictureRight.Image != originalImage && pictureRight.Image != currentRightImage)
            {
                pictureRight.Image.Dispose();
            }
        }
    }
}
