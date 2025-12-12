using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ImageProcessorWinForms
{
    public partial class MainForm : Form
    {
        // Controls
        private Button? btnLoad;
        private PictureBox? pbLeft;
        private PictureBox? pbRight;
        private Button? btnStep;
        private Button? btnStartStop;
        private Button? btnSaveLeft;
        private Button? btnSaveRight;
        private Button? btnSaveAll;
        private Label? lblStatus;
        private Label? lblLeftFilter;
        private Label? lblRightFilter;
        private ProgressBar? progressBar;
        private System.Windows.Forms.Timer? animationTimer;

        // Data
        private Bitmap? originalImage;
        private Bitmap? currentLeftImage;
        private Bitmap? currentRightImage;
        private int leftFilterIndex = 0;
        private int rightFilterIndex = 0;
        private bool isAnimating = false;
        private bool isProcessingLeft = false;
        private bool isProcessingRight = false;

        // Filters Enum
        private enum FilterType
        {
            Original,
            Grayscale,
            Invert,
            Brightness,
            Contrast,
            Sepia,
            Blur
        }

        private readonly FilterType[] filters = (FilterType[])Enum.GetValues(typeof(FilterType));

        public MainForm()
        {
            InitializeComponent();
            ApplyModernTheme();
        }

        private void InitializeComponent()
        {
            this.Text = "Image Processor - Modern Edition";
            this.Size = new Size(1200, 850);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Icon = null;

            // Main Layout
            TableLayoutPanel mainLayout = new TableLayoutPanel();
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.RowCount = 5;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));   // Header
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));   // Images
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));   // Progress
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));   // Controls
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));   // Status
            this.Controls.Add(mainLayout);

            // ===== 1. HEADER SECTION =====
            Panel headerPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15) };
            headerPanel.BackColor = Color.FromArgb(25, 25, 35);
            
            btnLoad = CreateStyledButton("📁 Load Image", Color.FromArgb(0, 150, 136), 220, 45);
            btnLoad.Click += BtnLoad_Click;
            btnLoad.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            headerPanel.Controls.Add(btnLoad);
            btnLoad.Location = new Point(15, 12);
            
            mainLayout.Controls.Add(headerPanel, 0, 0);

            // ===== 2. IMAGES SECTION =====
            TableLayoutPanel imagesPanel = new TableLayoutPanel();
            imagesPanel.Dock = DockStyle.Fill;
            imagesPanel.ColumnCount = 2;
            imagesPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            imagesPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            imagesPanel.Padding = new Padding(10);
            imagesPanel.BackColor = Color.FromArgb(30, 30, 40);
            
            pbLeft = CreateStyledPictureBox();
            pbRight = CreateStyledPictureBox();
            
            imagesPanel.Controls.Add(CreateImagePanel("🎯 Manual Mode (Step)", pbLeft, "Original"), 0, 0);
            imagesPanel.Controls.Add(CreateImagePanel("⚙️ Auto Mode (Start/Stop)", pbRight, "Original"), 1, 0);
            mainLayout.Controls.Add(imagesPanel, 0, 1);

            // ===== 3. PROGRESS BAR =====
            Panel progressPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10, 5, 10, 5), BackColor = Color.FromArgb(30, 30, 40) };
            progressBar = new ProgressBar();
            progressBar.Dock = DockStyle.Fill;
            progressBar.Visible = false;
            progressBar.ForeColor = Color.FromArgb(0, 150, 136);
            progressPanel.Controls.Add(progressBar);
            mainLayout.Controls.Add(progressPanel, 0, 2);

            // ===== 4. CONTROLS SECTION =====
            Panel controlsPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15), BackColor = Color.FromArgb(25, 25, 35) };
            
            // Left Controls
            FlowLayoutPanel leftControls = new FlowLayoutPanel { Dock = DockStyle.Left, FlowDirection = FlowDirection.LeftToRight, AutoSize = true };
            leftControls.WrapContents = false;
            btnStep = CreateStyledButton("⏭️ Step", Color.FromArgb(66, 133, 244), 100, 45);
            btnStep.Click += BtnStep_Click;
            btnSaveLeft = CreateStyledButton("💾 Save Left", Color.FromArgb(52, 168, 224), 120, 45);
            btnSaveLeft.Click += (s, e) => SaveImage(currentLeftImage, "Left_Image");
            leftControls.Controls.Add(btnStep);
            leftControls.Controls.Add(btnSaveLeft);
            controlsPanel.Controls.Add(leftControls);

            // Right Controls
            FlowLayoutPanel rightControls = new FlowLayoutPanel { Dock = DockStyle.Right, FlowDirection = FlowDirection.RightToLeft, AutoSize = true };
            rightControls.WrapContents = false;
            btnSaveRight = CreateStyledButton("💾 Save Right", Color.FromArgb(52, 168, 224), 120, 45);
            btnSaveRight.Click += (s, e) => SaveImage(currentRightImage, "Right_Image");
            btnStartStop = CreateStyledButton("▶️ Start", Color.FromArgb(0, 150, 136), 100, 45);
            btnStartStop.Click += BtnStartStop_Click;
            rightControls.Controls.Add(btnStartStop);
            rightControls.Controls.Add(btnSaveRight);
            controlsPanel.Controls.Add(rightControls);

            // Center - Save All
            btnSaveAll = CreateStyledButton("💾 Save All", Color.FromArgb(255, 152, 0), 120, 45);
            btnSaveAll.Click += BtnSaveAll_Click;
            btnSaveAll.Anchor = AnchorStyles.None;
            btnSaveAll.Location = new Point((controlsPanel.Width - 120) / 2, 12);
            controlsPanel.Controls.Add(btnSaveAll);

            mainLayout.Controls.Add(controlsPanel, 0, 3);

            // ===== 5. STATUS SECTION =====
            Panel statusPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15, 5, 15, 5), BackColor = Color.FromArgb(20, 20, 30) };
            statusPanel.BorderStyle = BorderStyle.FixedSingle;
            
            lblStatus = new Label { Text = "✓ Ready", Dock = DockStyle.Left, TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.FromArgb(76, 175, 80), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            statusPanel.Controls.Add(lblStatus);
            
            mainLayout.Controls.Add(statusPanel, 0, 4);

            // Timer
            animationTimer = new System.Windows.Forms.Timer();
            animationTimer.Interval = 1200;
            animationTimer.Tick += AnimationTimer_Tick;

            // Initial State
            ToggleControls(false);
        }

        private Panel CreateImagePanel(string title, PictureBox pb, string filterName)
        {
            Panel panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10), BackColor = Color.FromArgb(35, 35, 45) };
            panel.BorderStyle = BorderStyle.FixedSingle;

            // Title
            Label lblTitle = new Label { Text = title, Dock = DockStyle.Top, Height = 30, ForeColor = Color.FromArgb(100, 200, 255), Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            panel.Controls.Add(lblTitle);

            // PictureBox
            pb.Dock = DockStyle.Fill;
            pb.Margin = new Padding(0, 5, 0, 5);
            panel.Controls.Add(pb);

            // Filter Label
            Label lblFilter = new Label { Dock = DockStyle.Bottom, Height = 25, TextAlign = ContentAlignment.MiddleCenter, ForeColor = Color.FromArgb(150, 150, 150), Font = new Font("Segoe UI", 9) };
            lblFilter.Text = $"Filter: {filterName}";
            panel.Controls.Add(lblFilter);

            // Store reference
            if (title.Contains("Manual")) lblLeftFilter = lblFilter;
            else lblRightFilter = lblFilter;

            return panel;
        }

        private void ApplyModernTheme()
        {
            this.BackColor = Color.FromArgb(20, 20, 30);
            this.ForeColor = Color.White;
        }

        private PictureBox CreateStyledPictureBox()
        {
            return new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(45, 45, 60),
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        private Button CreateStyledButton(string text, Color baseColor, int width = 140, int height = 40)
        {
            Button btn = new Button
            {
                Text = text,
                FlatStyle = FlatStyle.Flat,
                BackColor = baseColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Size = new Size(width, height),
                Cursor = Cursors.Hand,
                Margin = new Padding(5)
            };
            
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(baseColor, 0.2f);
            btn.FlatAppearance.MouseOverBackColor = ControlPaint.Light(baseColor, 0.2f);
            
            return btn;
        }

        private void ToggleControls(bool enabled)
        {
            if (btnStep != null) btnStep.Enabled = enabled;
            if (btnStartStop != null) btnStartStop.Enabled = enabled;
            if (btnSaveLeft != null) btnSaveLeft.Enabled = enabled;
            if (btnSaveRight != null) btnSaveRight.Enabled = enabled;
            if (btnSaveAll != null) btnSaveAll.Enabled = enabled;
        }

        private void UpdateStartStopButtonState()
        {
            if (btnStartStop == null) return;
            
            if (isAnimating)
            {
                btnStartStop.Text = "⏹️ Stop";
                btnStartStop.BackColor = Color.FromArgb(244, 67, 54);
            }
            else
            {
                btnStartStop.Text = "▶️ Start";
                btnStartStop.BackColor = Color.FromArgb(0, 150, 136);
            }
        }

        // ===== EVENT HANDLERS =====

        private void BtnLoad_Click(object? sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp|All Files|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        if (isAnimating)
                        {
                            isAnimating = false;
                            animationTimer?.Stop();
                            UpdateStartStopButtonState();
                        }

                        originalImage?.Dispose();
                        originalImage = new Bitmap(ofd.FileName);

                        leftFilterIndex = 0;
                        rightFilterIndex = 0;
                        
                        currentLeftImage = new Bitmap(originalImage);
                        currentRightImage = new Bitmap(originalImage);

                        pbLeft!.Image = currentLeftImage;
                        pbRight!.Image = currentRightImage;

                        UpdateFilterLabels("Original", "Original");
                        ToggleControls(true);
                        UpdateStatus("✓ Image loaded successfully", Color.FromArgb(76, 175, 80));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading image:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        UpdateStatus("✗ Error loading image", Color.FromArgb(244, 67, 54));
                    }
                }
            }
        }

        private async void BtnStep_Click(object? sender, EventArgs e)
        {
            if (originalImage == null || isProcessingLeft || btnStep == null) return;

            try
            {
                isProcessingLeft = true;
                btnStep.Enabled = false;
                ShowProgress("Processing Left Image...");

                leftFilterIndex = (leftFilterIndex + 1) % filters.Length;
                FilterType filter = filters[leftFilterIndex];

                Bitmap processed = await Task.Run(() => ApplyFilter(originalImage, filter));
                
                currentLeftImage?.Dispose();
                currentLeftImage = processed;
                pbLeft!.Image = currentLeftImage;

                UpdateFilterLabels(filter.ToString(), null);
                UpdateStatus($"✓ Left: Applied {filter}", Color.FromArgb(76, 175, 80));
            }
            catch (Exception ex)
            {
                UpdateStatus($"✗ Error: {ex.Message}", Color.FromArgb(244, 67, 54));
            }
            finally
            {
                isProcessingLeft = false;
                btnStep.Enabled = true;
                HideProgress();
            }
        }

        private void BtnStartStop_Click(object? sender, EventArgs e)
        {
            if (originalImage == null) return;

            isAnimating = !isAnimating;
            UpdateStartStopButtonState();

            if (isAnimating)
            {
                animationTimer?.Start();
                UpdateStatus("▶ Animation started", Color.FromArgb(33, 150, 243));
            }
            else
            {
                animationTimer?.Stop();
                UpdateStatus("⏸ Animation paused", Color.FromArgb(255, 152, 0));
            }
        }

        private async void AnimationTimer_Tick(object? sender, EventArgs e)
        {
            if (originalImage == null || isProcessingRight) return;

            try
            {
                isProcessingRight = true;
                ShowProgress("Processing Right Image...");
                
                rightFilterIndex = (rightFilterIndex + 1) % filters.Length;
                FilterType filter = filters[rightFilterIndex];

                Bitmap processed = await Task.Run(() => ApplyFilter(originalImage, filter));

                currentRightImage?.Dispose();
                currentRightImage = processed;
                pbRight!.Image = currentRightImage;

                UpdateFilterLabels(null, filter.ToString());
            }
            catch (Exception ex)
            {
                UpdateStatus($"✗ Error: {ex.Message}", Color.FromArgb(244, 67, 54));
            }
            finally
            {
                isProcessingRight = false;
                HideProgress();
            }
        }

        private void BtnSaveAll_Click(object? sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string path = fbd.SelectedPath;
                        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                        
                        if (currentLeftImage != null)
                        {
                            string leftPath = Path.Combine(path, $"Left_{timestamp}.png");
                            currentLeftImage.Save(leftPath, ImageFormat.Png);
                        }
                        
                        if (currentRightImage != null)
                        {
                            string rightPath = Path.Combine(path, $"Right_{timestamp}.png");
                            currentRightImage.Save(rightPath, ImageFormat.Png);
                        }
                        
                        UpdateStatus("✓ All images saved successfully", Color.FromArgb(76, 175, 80));
                        MessageBox.Show("Images saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        UpdateStatus($"✗ Error saving images: {ex.Message}", Color.FromArgb(244, 67, 54));
                        MessageBox.Show($"Error saving images:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void SaveImage(Bitmap? img, string defaultName)
        {
            if (img == null) return;
            
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp";
                sfd.FileName = $"{defaultName}_{DateTime.Now:yyyyMMdd_HHmmss}";
                
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        img.Save(sfd.FileName);
                        UpdateStatus($"✓ Saved: {Path.GetFileName(sfd.FileName)}", Color.FromArgb(76, 175, 80));
                    }
                    catch (Exception ex)
                    {
                        UpdateStatus($"✗ Error: {ex.Message}", Color.FromArgb(244, 67, 54));
                        MessageBox.Show($"Error saving image:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void UpdateStatus(string message, Color color)
        {
            if (lblStatus != null)
            {
                lblStatus.Text = message;
                lblStatus.ForeColor = color;
            }
        }

        private void UpdateFilterLabels(string? leftFilter, string? rightFilter)
        {
            if (leftFilter != null && lblLeftFilter != null)
                lblLeftFilter.Text = $"Filter: {leftFilter}";
            if (rightFilter != null && lblRightFilter != null)
                lblRightFilter.Text = $"Filter: {rightFilter}";
        }

        private void ShowProgress(string message)
        {
            if (progressBar != null)
            {
                progressBar.Visible = true;
                progressBar.Style = ProgressBarStyle.Marquee;
            }
            UpdateStatus(message, Color.FromArgb(33, 150, 243));
        }

        private void HideProgress()
        {
            if (progressBar != null)
            {
                progressBar.Visible = false;
                progressBar.Style = ProgressBarStyle.Continuous;
            }
        }

        // ===== IMAGE PROCESSING =====

        private Bitmap ApplyFilter(Bitmap original, FilterType filter)
        {
            Bitmap bmp = new Bitmap(original);
            
            using (Graphics g = Graphics.FromImage(bmp))
            {
                ColorMatrix? colorMatrix = null;

                switch (filter)
                {
                    case FilterType.Grayscale:
                        colorMatrix = new ColorMatrix(new float[][] {
                            new float[] {.3f, .3f, .3f, 0, 0},
                            new float[] {.59f, .59f, .59f, 0, 0},
                            new float[] {.11f, .11f, .11f, 0, 0},
                            new float[] {0, 0, 0, 1, 0},
                            new float[] {0, 0, 0, 0, 1}
                        });
                        break;
                    case FilterType.Sepia:
                        colorMatrix = new ColorMatrix(new float[][] {
                            new float[] {.393f, .349f, .272f, 0, 0},
                            new float[] {.769f, .686f, .534f, 0, 0},
                            new float[] {.189f, .168f, .131f, 0, 0},
                            new float[] {0, 0, 0, 1, 0},
                            new float[] {0, 0, 0, 0, 1}
                        });
                        break;
                    case FilterType.Invert:
                        colorMatrix = new ColorMatrix(new float[][] {
                            new float[] {-1, 0, 0, 0, 0},
                            new float[] {0, -1, 0, 0, 0},
                            new float[] {0, 0, -1, 0, 0},
                            new float[] {0, 0, 0, 1, 0},
                            new float[] {1, 1, 1, 0, 1}
                        });
                        break;
                    case FilterType.Brightness:
                        float b = 0.2f;
                        colorMatrix = new ColorMatrix(new float[][] {
                            new float[] {1, 0, 0, 0, 0},
                            new float[] {0, 1, 0, 0, 0},
                            new float[] {0, 0, 1, 0, 0},
                            new float[] {0, 0, 0, 1, 0},
                            new float[] {b, b, b, 0, 1}
                        });
                        break;
                    case FilterType.Contrast:
                        float c = 1.5f;
                        float t = 0.5f * (1.0f - c);
                        colorMatrix = new ColorMatrix(new float[][] {
                            new float[] {c, 0, 0, 0, 0},
                            new float[] {0, c, 0, 0, 0},
                            new float[] {0, 0, c, 0, 0},
                            new float[] {0, 0, 0, 1, 0},
                            new float[] {t, t, t, 0, 1}
                        });
                        break;
                    case FilterType.Blur:
                        // Blur is more complex and requires direct pixel manipulation, 
                        // which is not easily done with ColorMatrix. 
                        // For simplicity and to avoid external libraries, we will skip 
                        // the full implementation here, but the structure is ready.
                        // For now, we will return the original bitmap.
                        return ApplyBlurFilter(original);
                    case FilterType.Original:
                    default:
                        return bmp;
                }

                if (colorMatrix != null)
                {
                    ImageAttributes attributes = new ImageAttributes();
                    attributes.SetColorMatrix(colorMatrix);
                    g.DrawImage(original, new Rectangle(0, 0, bmp.Width, bmp.Height),
                        0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
                }
            }
            
            return bmp;
        }

        // Simple Box Blur Implementation (Pixel-based)
        private Bitmap ApplyBlurFilter(Bitmap sourceBitmap)
        {
            int width = sourceBitmap.Width;
            int height = sourceBitmap.Height;
            Bitmap blurredBitmap = new Bitmap(width, height);

            // Lock the bitmap bits
            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData srcData = sourceBitmap.LockBits(rect, ImageLockMode.ReadOnly, sourceBitmap.PixelFormat);
            BitmapData dstData = blurredBitmap.LockBits(rect, ImageLockMode.WriteOnly, blurredBitmap.PixelFormat);

            int bytesPerPixel = Image.GetPixelFormatSize(sourceBitmap.PixelFormat) / 8;
            int stride = srcData.Stride;

            IntPtr srcPtr = srcData.Scan0;
            IntPtr dstPtr = dstData.Scan0;

            // Byte arrays for pixel data
            byte[] srcBytes = new byte[height * stride];
            byte[] dstBytes = new byte[height * stride];

            // Copy data from unmanaged memory to managed array
            System.Runtime.InteropServices.Marshal.Copy(srcPtr, srcBytes, 0, srcBytes.Length);

            int kernelSize = 5; // 5x5 box blur
            int kernelOffset = (kernelSize - 1) / 2;
            int totalPixels = kernelSize * kernelSize;

            for (int y = kernelOffset; y < height - kernelOffset; y++)
            {
                for (int x = kernelOffset; x < width - kernelOffset; x++)
                {
                    int red = 0;
                    int green = 0;
                    int blue = 0;

                    // Iterate over the kernel
                    for (int ky = -kernelOffset; ky <= kernelOffset; ky++)
                    {
                        for (int kx = -kernelOffset; kx <= kernelOffset; kx++)
                        {
                            int currentX = x + kx;
                            int currentY = y + ky;

                            int index = currentY * stride + currentX * bytesPerPixel;

                            // Assuming 32bpp (BGRA) or 24bpp (BGR)
                            if (bytesPerPixel >= 3)
                            {
                                blue += srcBytes[index];
                                green += srcBytes[index + 1];
                                red += srcBytes[index + 2];
                            }
                        }
                    }

                    // Calculate average
                    int avgRed = red / totalPixels;
                    int avgGreen = green / totalPixels;
                    int avgBlue = blue / totalPixels;

                    // Write to destination
                    int dstIndex = y * stride + x * bytesPerPixel;
                    if (bytesPerPixel >= 3)
                    {
                        dstBytes[dstIndex] = (byte)avgBlue;
                        dstBytes[dstIndex + 1] = (byte)avgGreen;
                        dstBytes[dstIndex + 2] = (byte)avgRed;
                        if (bytesPerPixel == 4)
                        {
                            dstBytes[dstIndex + 3] = srcBytes[dstIndex + 3]; // Keep Alpha channel
                        }
                    }
                }
            }

            // Copy data back to unmanaged memory
            System.Runtime.InteropServices.Marshal.Copy(dstBytes, 0, dstPtr, dstBytes.Length);

            // Unlock the bits
            sourceBitmap.UnlockBits(srcData);
            blurredBitmap.UnlockBits(dstData);

            return blurredBitmap;
        }
    }
}
