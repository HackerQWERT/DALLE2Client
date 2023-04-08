namespace DALLE2Client.Views;

public partial class TestView : ContentPage
{
    public TestView(TestViewModel testViewModel)
	{
        InitializeComponent();
        BindingContext=testViewModel;                   
    }
    public class PhotoGroupTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TimeTemplate { get; set; }
        public DataTemplate PhotoTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return item switch
            {
                _ => null,
            };
        }
    }

}