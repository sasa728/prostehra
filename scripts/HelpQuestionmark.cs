using Godot;
using System;

public partial class HelpQuestionmark : TextureRect
{
	// Odkaz na okno nápovědy
	private Panel helpWindow;
	private Button gotItButton;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Získání panelu nápovědy a tlačítka
		helpWindow = GetNode<Panel>("HelpPanel");
		gotItButton = GetNode<Button>("HelpPanel/GotItButton");



		// Ujistíme se, že panel je na začátku skrytý
		helpWindow.Visible = false;

		// Připojení signálu pro stisknutí tlačítka "Got It"
		gotItButton.Pressed += OnGotItButtonPressed;
	}

	// Funkce pro detekci kliknutí na ikonu otazníku
	public override void _Input(InputEvent @event)
	{
		// Kontrola, zda událost je kliknutí levým tlačítkem myši
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.Pressed)
		{
			// Pokud klikneme na ikonu otazníku, zobrazíme panel
			if (GetRect().HasPoint(mouseEvent.Position))
			{
				helpWindow.Visible = true; // Zobrazíme panel s nápovědou
			}
		}
	}

	// Funkce pro skrytí panelu po kliknutí na tlačítko "Got It"
	private void OnGotItButtonPressed()
	{
		helpWindow.Visible = false; // Skryjeme panel nápovědy
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
