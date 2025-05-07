using Godot;
using System;

public partial class ExitButton : Button
{
	public override void _Ready()
	{
		this.Pressed += OnExitButtonPressed;
		
	}

	private void OnExitButtonPressed()
	{
		// Zavře hru
		GetTree().Quit();
	}

	public override void _Process(double delta)
	{
	}
}
