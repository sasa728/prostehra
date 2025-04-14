using Godot;
using System;

public partial class Battle : Control
{
	// 1. DATA HRÁČE
	public class PlayerStats
	{
		public int Attack = 10;
		public int Defense = 5;
		public int MaxVitality = 100;
		public int CurrentVitality = 50;
		public int Luck = 1;
		public int Coins = 50;
		public int Interest = 0;
		public int Lives = 3;
	}

	private PlayerStats _player = new PlayerStats();

	// 2. REFERENCE NA UI ELEMENTY
	private Label _attackLabel;
	private Label _defenseLabel;
	private Label _vitalityLabel;
	private Label _luckLabel;
	private ProgressBar _hpBar;
	private Label _hpValueLabel;
	private Label _coinsLabel;
	private Label _interestLabel;

	public override void _Ready()
	{
		// 3. NAJDĚTE UI PODLE TVÉHO STROMU
		_attackLabel = GetNode<Label>("PlayerPanel/PlayerData/PlayerStats/PlayerAttack");
		_defenseLabel = GetNode<Label>("PlayerPanel/PlayerData/PlayerStats/PlayerDefense");
		_vitalityLabel = GetNode<Label>("PlayerPanel/PlayerData/PlayerStats/PlayerVitality");
		_luckLabel = GetNode<Label>("PlayerPanel/PlayerData/PlayerStats/PlayerLuck");
		_hpBar = GetNode<ProgressBar>("PlayerPanel/PlayerData/PlayerStats/ProgressBar");
		_hpValueLabel = GetNode<Label>("PlayerPanel/PlayerData/PlayerStats/ProgressBar/HpLabel");
		_coinsLabel = GetNode<Label>("PlayerPanel/PlayerData/PlayerMoney/PlayerCoins");
		_interestLabel = GetNode<Label>("PlayerPanel/PlayerData/PlayerMoney/PlayerInterrest");

		// 4. TEST: ZMĚNA STATŮ A AKTUALIZACE UI
		_player.Attack += 5;
		_player.CurrentVitality -= 30;
		_player.Coins = 100;
		RefreshUI();
	}

	// 5. METODA PRO AKTUALIZACI CELÉHO UI
	private void RefreshUI()
	{
		// Staty hráče
		_attackLabel.Text = $"Attack: {_player.Attack}";
		_defenseLabel.Text = $"Defense: {_player.Defense}";
		_vitalityLabel.Text = $"Vitality: {_player.MaxVitality}";
		_luckLabel.Text = $"Luck: {_player.Luck}";
		
		// Zdraví
		_hpBar.MaxValue = _player.MaxVitality;
		_hpBar.Value = _player.CurrentVitality;
		_hpValueLabel.Text = $"{_player.CurrentVitality}/{_player.MaxVitality}";
		
		// Peníze
		_coinsLabel.Text = $"Coins: {_player.Coins}";
		_interestLabel.Text = $"Interest: {_player.Interest}%";
	}
}
