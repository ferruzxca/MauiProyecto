namespace MauiProyecto
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            // Validar si los campos de usuario y contraseña están completos
            if (string.IsNullOrEmpty(UsernameEntry.Text) || string.IsNullOrEmpty(PasswordEntry.Text))
            {
                await DisplayAlert("Error", "Por favor ingrese su usuario y contraseña.", "OK");
                return;
            }

            // Validar si el usuario aceptó los términos
            if (!TermsAcceptedRadio.IsChecked)
            {
                await DisplayAlert("Error", "Debe aceptar las condiciones de la aplicación para continuar.", "OK");
                return;
            }

            // Validar si el usuario es mayor de edad
            if (!AgeAcceptedRadio.IsChecked)
            {
                await DisplayAlert("Error", "Debe ser mayor de edad para continuar.", "OK");
                return;
            }

            // Si todo está bien, proceder al login (navegar a otra página, por ejemplo)
            await DisplayAlert("Bienvenido", $"Hola, {UsernameEntry.Text}!", "OK");

            // Aquí podrías navegar a una nueva página
            await Navigation.PushAsync(new EspanolPage());
        }
    }
}
