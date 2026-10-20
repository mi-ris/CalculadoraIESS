
using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace CalculadoraIESS;

public partial class MainPage : ContentPage
{
    //private readonly Color colorNormal =
        Color.FromArgb("#DCE4F0");

    private readonly Color colorActivo =
        Color.FromArgb("#3155D9");

    public MainPage()
    {
        InitializeComponent();
    }

    // Resaltar el campo que está seleccionado
    private void Campo_Focused(object sender, FocusEventArgs e)
    {
        NombreBox.Stroke = colorNormal;
        ApellidoBox.Stroke = colorNormal;
        EdadBox.Stroke = colorNormal;
        SalarioBox.Stroke = colorNormal;

        if (sender == NombreEntry)
            NombreBox.Stroke = colorActivo;
        else if (sender == ApellidoEntry)
            ApellidoBox.Stroke = colorActivo;
        else if (sender == EdadEntry)
            EdadBox.Stroke = colorActivo;
        else if (sender == SalarioEntry)
            SalarioBox.Stroke = colorActivo;

        if (sender is VisualElement elemento)
        {
            Dispatcher.Dispatch(async () =>
            {
                await Task.Delay(250);

                await FormScroll.ScrollToAsync(
                    elemento,
                    ScrollToPosition.MakeVisible,
                    true);
            });
        }
    }

    // Restaurar el borde al salir del campo
    private void Campo_Unfocused(object sender, FocusEventArgs e)
    {
        if (sender == NombreEntry)
            NombreBox.Stroke = colorNormal;
        else if (sender == ApellidoEntry)
            ApellidoBox.Stroke = colorNormal;
        else if (sender == EdadEntry)
            EdadBox.Stroke = colorNormal;
        else if (sender == SalarioEntry)
            SalarioBox.Stroke = colorNormal;
    }

    // Navegación entre los campos
    private void Nombre_Completed(object sender, EventArgs e)
    {
        ApellidoEntry.Focus();
    }

    private void Apellido_Completed(object sender, EventArgs e)
    {
        EdadEntry.Focus();
    }

    private void Edad_Completed(object sender, EventArgs e)
    {
        SalarioEntry.Focus();
    }

    private async void Salario_Completed(object sender, EventArgs e)
    {
        await CalcularAporte();
    }

    // Evento del botón
    private async void CalcularAporte_Clicked(
        object sender, EventArgs e)
    {
        await CalcularAporte();
    }

    private async Task CalcularAporte()
    {
        // 1. Comprobar campos obligatorios
        if (string.IsNullOrWhiteSpace(NombreEntry.Text) ||
            string.IsNullOrWhiteSpace(ApellidoEntry.Text) ||
            string.IsNullOrWhiteSpace(EdadEntry.Text) ||
            string.IsNullOrWhiteSpace(SalarioEntry.Text))
        {
            await DisplayAlert(
                "Datos incompletos",
                "Por favor, completa los cuatro campos.",
                "Entendido");

            return;
        }

        // 2. Validar edad
        if (!int.TryParse(EdadEntry.Text.Trim(), out int edad) ||
            edad < 1 || edad > 120)
        {
            await DisplayAlert(
                "Edad inválida",
                "Ingresa una edad entera entre 1 y 120.",
                "Entendido");

            EdadEntry.Focus();
            return;
        }

        // 3. Validar salario con punto o coma decimal
        string textoSalario = SalarioEntry.Text.Trim();

        bool salarioValido = decimal.TryParse(
            textoSalario,
            NumberStyles.Number,
            CultureInfo.CurrentCulture,
            out decimal salario);

        if (!salarioValido)
        {
            salarioValido = decimal.TryParse(
                textoSalario,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out salario);
        }

        if (!salarioValido || salario < 0)
        {
            await DisplayAlert(
                "Salario inválido",
                "Ingresa un salario numérico igual o mayor a cero.",
                "Entendido");

            SalarioEntry.Focus();
            return;
        }

        // 4. Calcular el 9,45 %
        decimal aporte = salario * 0.0945m;

        // 5. Preparar los datos
        string nombre = NombreEntry.Text.Trim();
        string apellido = ApellidoEntry.Text.Trim();

        string aporteTexto = aporte.ToString(
            "N2",
            CultureInfo.GetCultureInfo("es-EC"));

        string mensaje =
            $"Bienvenido {nombre} {apellido}\n" +
            $"Tienes {edad} años\n" +
            $"Tu aporte mensual es ${aporteTexto}";

        // 6. Mostrar resultado
        await DisplayAlert(
            "Resultado del IESS",
            mensaje,
            "Aceptar");
    }
}
