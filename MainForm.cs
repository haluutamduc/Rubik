using System;
using System.Windows.Forms;
using OpenTK.WinForms;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace RubikSolver
{
    public class MainForm : Form
    {
        private GLControl glControl;
        private System.Windows.Forms.Timer renderTimer;  // Chỉ rõ namespace

        public MainForm()
        {
            InitializeComponents();
            SetupTimer();
        }

        // Thêm biến cho điều khiển chuột
        private bool isMouseDown = false;
        private Point lastMousePos;
        private const float ROTATION_SPEED = 0.01f;

        private void InitializeComponents()
        {
            this.Text = "Rubik Solver";
            this.Size = new System.Drawing.Size(800, 600);

            glControl = new GLControl();
            glControl.Dock = DockStyle.Fill;
            glControl.Load += GLControl_Load;
            glControl.Paint += GLControl_Paint;
            glControl.Resize += GLControl_Resize;
            
            // Thêm các sự kiện chuột
            glControl.MouseDown += GLControl_MouseDown;
            glControl.MouseMove += GLControl_MouseMove;
            glControl.MouseUp += GLControl_MouseUp;

            this.Controls.Add(glControl);
        }

        private void SetupTimer()
        {
            renderTimer = new System.Windows.Forms.Timer();
            renderTimer.Interval = 16; // Khoảng 60 FPS
            renderTimer.Tick += (sender, e) => glControl.Invalidate();
            renderTimer.Start();
        }

        // Thêm các biến cho camera và góc xoay
        private float cameraDistance = 5.0f;
        private Vector3 cameraPosition;
        private float rotationX = 0.0f;
        private float rotationY = 0.0f;
        private Matrix4 projectionMatrix;
        private Matrix4 viewMatrix;

        private void GLControl_Load(object sender, EventArgs e)
        {
            GL.ClearColor(Color4.DarkGray);
            GL.Enable(EnableCap.DepthTest);
            
            // Thiết lập camera
            UpdateCamera();
            
            // Thiết lập phép chiếu phối cảnh
            float aspectRatio = glControl.Width / (float)glControl.Height;
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(45.0f),
                aspectRatio,
                0.1f,
                100.0f
            );
        }

        private void UpdateCamera()
        {
            // Tính toán vị trí camera
            cameraPosition = new Vector3(
                cameraDistance * (float)Math.Sin(rotationY) * (float)Math.Cos(rotationX),
                cameraDistance * (float)Math.Sin(rotationX),
                cameraDistance * (float)Math.Cos(rotationY) * (float)Math.Cos(rotationX)
            );
            
            // Cập nhật ma trận view
            viewMatrix = Matrix4.LookAt(
                cameraPosition,
                Vector3.Zero,
                Vector3.UnitY
            );
        }

        private void DrawCube()
        {
            GL.Begin(PrimitiveType.Quads);

            // Mặt trước (Đỏ)
            GL.Color3(1.0f, 0.0f, 0.0f);
            GL.Vertex3(-0.5f, -0.5f, 0.5f);
            GL.Vertex3(0.5f, -0.5f, 0.5f);
            GL.Vertex3(0.5f, 0.5f, 0.5f);
            GL.Vertex3(-0.5f, 0.5f, 0.5f);

            // Mặt sau (Cam)
            GL.Color3(1.0f, 0.5f, 0.0f);
            GL.Vertex3(-0.5f, -0.5f, -0.5f);
            GL.Vertex3(-0.5f, 0.5f, -0.5f);
            GL.Vertex3(0.5f, 0.5f, -0.5f);
            GL.Vertex3(0.5f, -0.5f, -0.5f);

            // Mặt trên (Trắng)
            GL.Color3(1.0f, 1.0f, 1.0f);
            GL.Vertex3(-0.5f, 0.5f, -0.5f);
            GL.Vertex3(-0.5f, 0.5f, 0.5f);
            GL.Vertex3(0.5f, 0.5f, 0.5f);
            GL.Vertex3(0.5f, 0.5f, -0.5f);

            // Mặt dưới (Vàng)
            GL.Color3(1.0f, 1.0f, 0.0f);
            GL.Vertex3(-0.5f, -0.5f, -0.5f);
            GL.Vertex3(0.5f, -0.5f, -0.5f);
            GL.Vertex3(0.5f, -0.5f, 0.5f);
            GL.Vertex3(-0.5f, -0.5f, 0.5f);

            // Mặt phải (Xanh lá)
            GL.Color3(0.0f, 1.0f, 0.0f);
            GL.Vertex3(0.5f, -0.5f, -0.5f);
            GL.Vertex3(0.5f, 0.5f, -0.5f);
            GL.Vertex3(0.5f, 0.5f, 0.5f);
            GL.Vertex3(0.5f, -0.5f, 0.5f);

            // Mặt trái (Xanh dương)
            GL.Color3(0.0f, 0.0f, 1.0f);
            GL.Vertex3(-0.5f, -0.5f, -0.5f);
            GL.Vertex3(-0.5f, -0.5f, 0.5f);
            GL.Vertex3(-0.5f, 0.5f, 0.5f);
            GL.Vertex3(-0.5f, 0.5f, -0.5f);

            GL.End();
        }

        private void GLControl_Paint(object sender, PaintEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            // Áp dụng các ma trận transform
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projectionMatrix);
            
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref viewMatrix);
            
            // Vẽ khối lập phương
            DrawCube();
            
            glControl.SwapBuffers();
        }

        private void GLControl_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, glControl.Width, glControl.Height);
            
            // Cập nhật ma trận phép chiếu khi thay đổi kích thước
            float aspectRatio = glControl.Width / (float)glControl.Height;
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(45.0f),
                aspectRatio,
                0.1f,
                100.0f
            );
        }

        private void GLControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = true;
                lastMousePos = e.Location;
            }
        }

        private void GLControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                float deltaX = (e.X - lastMousePos.X) * ROTATION_SPEED;
                float deltaY = (e.Y - lastMousePos.Y) * ROTATION_SPEED;

                rotationY += deltaX;
                rotationX += deltaY;

                // Giới hạn góc xoay theo trục X để tránh lật ngược camera
                rotationX = Math.Max(-1.5f, Math.Min(1.5f, rotationX));

                UpdateCamera();
                lastMousePos = e.Location;
            }
        }

        private void GLControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = false;
            }
        }
    }
}