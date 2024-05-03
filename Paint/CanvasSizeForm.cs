using System.Windows.Forms;

namespace Paint
{
    public partial class CanvasSizeForm : Form
    {
        public CanvasSizeForm(DocumentForm activeDocumentForm)
        {
            InitializeComponent();
            WidthTextBox.Text = activeDocumentForm.Width.ToString();
            HeightTextBox.Text = activeDocumentForm.Height.ToString();
        }

        public string GetWidth()
        {
            return WidthTextBox.Text;
        }

        public string GetHeight()
        {
            return HeightTextBox.Text;
        }
    }
}
