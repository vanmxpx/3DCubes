using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _3DCubes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const double LINE_THICKNESS = 0.003;
        public MainWindow()
        {
            InitializeComponent();
        }
        private Vector3D CreateNormal(Point3D p0, Point3D p1, Point3D p2)
        {
            Vector3D v0 = new Vector3D(p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
            Vector3D v1 = new Vector3D(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
            return Vector3D.CrossProduct(v0, v1);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MeshGeometry3D lbl_mesh = new MeshGeometry3D();
            ModelVisual3D lbl_model = new ModelVisual3D();
            CreateLabels(lbl_mesh, lbl_model, "TESTIKIITTT", 10, new Point3D(0, 0.25, 1.25));


            //Create verticals
            List<Point3D> points = new List<Point3D>();
            points.Add(new Point3D(1, 1, 1));
            points.Add(new Point3D(1, 1, 0));
            points.Add(new Point3D(0, 1, 0));
            points.Add(new Point3D(0, 1, 1));

            points.Add(new Point3D(1, 0, 1));
            points.Add(new Point3D(1, 0, 0));
            points.Add(new Point3D(0, 0, 0));
            points.Add(new Point3D(0, 0, 1));

            //Create axes
            MeshGeometry3D axes_mesh = new MeshGeometry3D();
            Point3D origin = new Point3D(0, 0, 0);
            Point3D xmax = new Point3D(1.25, 0, 0);
            Point3D ymax = new Point3D(0, 1.25, 0);
            Point3D zmax = new Point3D(0, 0, 1.25);
            CreateRect3D(axes_mesh, origin, xmax, new Vector3D(0, 1, 0));
            CreateRect3D(axes_mesh, origin, zmax, new Vector3D(0, 1, 0));
            CreateRect3D(axes_mesh, origin, ymax, new Vector3D(1, 0, 0));

            SolidColorBrush axes_brush = Brushes.Red;
            DiffuseMaterial axes_material = new DiffuseMaterial(axes_brush);
            GeometryModel3D axes_model = new GeometryModel3D(axes_mesh, axes_material);

            // Create the solid cube.
            MeshGeometry3D solid_mesh = new MeshGeometry3D();
            CreateRect2D(solid_mesh, points[0], points[1], points[2], points[3]);
            CreateRect2D(solid_mesh, points[0], points[4], points[5], points[1]);
            CreateRect2D(solid_mesh, points[1], points[5], points[6], points[2]);
            CreateRect2D(solid_mesh, points[2], points[6], points[7], points[3]);
            CreateRect2D(solid_mesh, points[3], points[7], points[4], points[0]);
            CreateRect2D(solid_mesh, points[7], points[6], points[5], points[4]);

            SolidColorBrush solid_brush = new SolidColorBrush(Color.FromArgb(100, 200, 200, 200));
            DiffuseMaterial solid_material = new DiffuseMaterial(solid_brush);
            GeometryModel3D solid_model = new GeometryModel3D(solid_mesh, solid_material);

            Model3DGroup mainModelGroup = new Model3DGroup();
            mainModelGroup.Children.Add(axes_model);
            mainModelGroup.Children.Add(solid_model);



            // Add the group of models to a ModelVisual3D.
            ModelVisual3D model_visual = new ModelVisual3D();
            model_visual.Content = mainModelGroup;

            // Display the main visual to the viewportt.
            MainViewport.Children.Add(model_visual);
            MainViewport.Children.Add(lbl_model);
        }

        private void CreateLabels(MeshGeometry3D mesh, ModelVisual3D lbl_model, string text, int height, Point3D basePoint)
        {

            //CreateLabels
            TextBlock textblock = new TextBlock(new Run(text));
            textblock.Foreground = Brushes.AliceBlue; // setting the text color

            Vector3D vectorUp = new Vector3D(1, 1, 0);
            Vector3D vectorOver = new Vector3D(1, 0, 1);
            DiffuseMaterial mataterialWithLabel = new DiffuseMaterial();
            // Allows the application of a 2-D brush,
            // like a SolidColorBrush or TileBrush, to a diffusely-lit 3-D model.
            // we are creating the brush from the TextBlock
            mataterialWithLabel.Brush = new VisualBrush(textblock);

            //calculation of text width (assuming that characters are square):
            double width = text.Length * height;
            // we need to find the four corners
            // p0: the lower left corner; p1: the upper left
            // p2: the lower right; p3: the upper right
            Point3D p0 = basePoint;
                
            Point3D p1 = p0 + vectorUp * 1 * height;
            Point3D p2 = p0 + vectorOver * width;
            Point3D p3 = p0 + vectorUp * 1 * height + vectorOver * width;
            // we are going to create object in 3D now:
            // this object will be painted using the (text) brush created before
            // the object is rectangle made of two triangles (on each side).

            mesh.Positions.Add(p0); // 0
            mesh.Positions.Add(p1); // 1
            mesh.Positions.Add(p2); // 2
            mesh.Positions.Add(p3); // 3

                //Revert side
                mesh.Positions.Add(p0); // 4
                mesh.Positions.Add(p1); // 5
                mesh.Positions.Add(p2); // 6
                mesh.Positions.Add(p3); // 7

            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(3);

                //Revert side
                mesh.TriangleIndices.Add(4);
                mesh.TriangleIndices.Add(5);
                mesh.TriangleIndices.Add(7);
                mesh.TriangleIndices.Add(4);
                mesh.TriangleIndices.Add(7);
                mesh.TriangleIndices.Add(6);

            // texture coordinates must be set to
            // stretch the TextBox brush to cover
            // the full side of the 3D label.
            mesh.TextureCoordinates.Add(new Point(0, 1));
            mesh.TextureCoordinates.Add(new Point(0, 0));
            mesh.TextureCoordinates.Add(new Point(1, 1));
            mesh.TextureCoordinates.Add(new Point(1, 0));

                //label is double sided:
                mesh.TextureCoordinates.Add(new Point(1, 1));
                mesh.TextureCoordinates.Add(new Point(1, 0));
                mesh.TextureCoordinates.Add(new Point(0, 1));
                mesh.TextureCoordinates.Add(new Point(0, 0));

            lbl_model.Content = new GeometryModel3D(mesh, mataterialWithLabel); ;
        }

        private void CreateRect3D(MeshGeometry3D mesh, Point3D from, Point3D to, Vector3D expand)
        {
            // Get the line.
            Vector3D v = to - from;

            // Get the scaled up vector.
            Vector3D n1 = ScaleVector(expand, LINE_THICKNESS);

            // Get mirrored vector.
            Vector3D n2 = Vector3D.CrossProduct(v, n1);
            n2 = ScaleVector(n2, LINE_THICKNESS);

            // Make a skinny box.
            // p1pm means point1 PLUS n1 MINUS n2.
            Point3D p1pp = from + n1 + n2;
            Point3D p1mp = from - n1 + n2;
            Point3D p1pm = from + n1 - n2;
            Point3D p1mm = from - n1 - n2;
            Point3D p2pp = to + n1 + n2;
            Point3D p2mp = to - n1 + n2;
            Point3D p2pm = to + n1 - n2;
            Point3D p2mm = to - n1 - n2;

            // Sides.
            AddTriangle(mesh, p1pp, p1mp, p2mp);
            AddTriangle(mesh, p1pp, p2mp, p2pp);

            AddTriangle(mesh, p1pp, p2pp, p2pm);
            AddTriangle(mesh, p1pp, p2pm, p1pm);

            AddTriangle(mesh, p1pm, p2pm, p2mm);
            AddTriangle(mesh, p1pm, p2mm, p1mm);

            AddTriangle(mesh, p1mm, p2mm, p2mp);
            AddTriangle(mesh, p1mm, p2mp, p1mp);

            // Ends.
            AddTriangle(mesh, p1pp, p1pm, p1mm);
            AddTriangle(mesh, p1pp, p1mm, p1mp);

            AddTriangle(mesh, p2pp, p2mp, p2mm);
            AddTriangle(mesh, p2pp, p2mm, p2pm);
        }

        // Add a rectangle to the indicated mesh.
        // Do not reuse existing points but reuse these points
        // so new rectangles don't share normals with old ones.
        private void CreateRect2D(MeshGeometry3D mesh,
            Point3D point1, Point3D point2, Point3D point3, Point3D point4)
        {
            // Create the points.
            int index1 = mesh.Positions.Count;
            mesh.Positions.Add(point1);
            mesh.Positions.Add(point2);
            mesh.Positions.Add(point3);
            mesh.Positions.Add(point4);

            // Create the triangles.
            mesh.TriangleIndices.Add(index1);
            mesh.TriangleIndices.Add(index1 + 1);
            mesh.TriangleIndices.Add(index1 + 2);

            mesh.TriangleIndices.Add(index1);
            mesh.TriangleIndices.Add(index1 + 2);
            mesh.TriangleIndices.Add(index1 + 3);
        }

        #region Tools
        private Vector3D ScaleVector(Vector3D vector, double length)
        {
            double scale = length / vector.Length;
            return new Vector3D(
                vector.X * scale,
                vector.Y * scale,
                vector.Z * scale);
        }
        private void AddTriangle(MeshGeometry3D mesh, Point3D point1, Point3D point2, Point3D point3)
        {
            // Create the points.
            int index1 = mesh.Positions.Count;
            mesh.Positions.Add(point1);
            mesh.Positions.Add(point2);
            mesh.Positions.Add(point3);

            // Create the triangle.
            mesh.TriangleIndices.Add(index1++);
            mesh.TriangleIndices.Add(index1++);
            mesh.TriangleIndices.Add(index1);
        }
        #endregion
    }
}
