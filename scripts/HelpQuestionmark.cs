using Godot;
using System;

public partial class HelpQuestionmark : TextureRect
{
	private Panel helpWindow;
	private Button gotItButton;

	public override void _Ready()
	{
		helpWindow = GetNode<Panel>("HelpPanel");
		gotItButton = GetNode<Button>("HelpPanel/GotItButton");
		helpWindow.Visible = false;
		gotItButton.Pressed += OnGotItButtonPressed;
	}

	// Funkce pro detekci kliknutí na ikonu otazníku
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.Pressed)
		{
			// Pokud klikneme na ikonu otazníku, zobrazíme panel
			if (GetRect().HasPoint(mouseEvent.Position))
			{
				helpWindow.Visible = true; 
			}
		}
	}

	// Funkce pro skrytí panelu po kliknutí na tlačítko "Got It"
	private void OnGotItButtonPressed()
	{
		helpWindow.Visible = false;
	}

	public override void _Process(double delta)
	{
	}
}
