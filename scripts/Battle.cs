using Godot;
using System;

public partial class Battle : Control
{
	// 1. DATA HRÁČE A NEPŘÍTELE
	public class PlayerStats
	{
		public int Attack = 10;
		public int Defense = 5;
		public int Vitality = 10;
		public int CurrentVitality;
		public int Luck = 1;
		public int Coins = 50;
		public int Lives = 3;

		public int MaxVitality => Vitality * 10;

		public PlayerStats()
		{
			CurrentVitality = MaxVitality;
		}
	}

	public class EnemyStats
	{
		public int Attack;
		public int Defense;
		public int MaxVitality;
		public int CurrentVitality;
	}

	private PlayerStats _player = new PlayerStats();
	private EnemyStats _enemy = new EnemyStats();
	private int _currentRound = 1;
	private Random _rnd = new Random();

	// 2. UI REFERENCE
	private Label _attackLabel, _defenseLabel, _vitalityLabel, _luckLabel;
	private ProgressBar _hpBar, _enemyHpBar;
	private Label _hpValueLabel, _enemyHpLabel;
	private Label _coinsLabel, _interestLabel;
	private Label _enemyAttackLabel, _enemyDefenseLabel;
	private TextureRect _enemyTexture;
	private Button _fightButton;
	private TextureRect[] _hearts;

	public override void _Ready()
	{
		InitializeUIReferences();
		GenerateNewEnemy();
		RefreshUI();

		_fightButton = GetNode<Button>("EnemyContainer/FightButton");
		_fightButton.Pressed += OnFightButtonPressed;

		_hearts = new TextureRect[3]
		{
			GetNode<TextureRect>("LivesPanel/Live1"),
			GetNode<TextureRect>("LivesPanel/Live2"),
			GetNode<TextureRect>("LivesPanel/Live3")
		};
		UpdateHeartsUI();
	}

	private void InitializeUIReferences()
	{
		_attackLabel = GetNode<Label>("PlayerPanel/PlayerData/PlayerStats/PlayerAttack");
		_defenseLabel = GetNode<Label>("PlayerPanel/PlayerData/PlayerStats/PlayerDefense");
		_vitalityLabel = GetNode<Label>("PlayerPanel/PlayerData/PlayerStats/PlayerVitality");
		_luckLabel = GetNode<Label>("PlayerPanel/PlayerData/PlayerStats/PlayerLuck");
		_hpBar = GetNode<ProgressBar>("PlayerPanel/PlayerData/PlayerStats/ProgressBar");
		_hpValueLabel = GetNode<Label>("PlayerPanel/PlayerData/PlayerStats/ProgressBar/HpLabel");
		_coinsLabel = GetNode<Label>("PlayerPanel/PlayerData/PlayerMoney/PlayerCoins");
		_interestLabel = GetNode<Label>("PlayerPanel/PlayerData/PlayerMoney/PlayerInterrest");

		_enemyAttackLabel = GetNode<Label>("PlayerPanel/PlayerData/EnemyStats/EnemyAttack");
		_enemyDefenseLabel = GetNode<Label>("PlayerPanel/PlayerData/EnemyStats/EnemyDefense");
		_enemyHpBar = GetNode<ProgressBar>("EnemyContainer/ProgressBar");
		_enemyHpLabel = GetNode<Label>("EnemyContainer/ProgressBar/HpLabel");
		_enemyTexture = GetNode<TextureRect>("EnemyContainer/Enemy");
	}

	private void GenerateNewEnemy()
	{
		_enemy.Attack = 5 + _currentRound * 2;
		_enemy.Defense = 3 + _currentRound;
		_enemy.MaxVitality = 50 + _currentRound * 15;
		_enemy.CurrentVitality = _enemy.MaxVitality;

		var enemyNumber = _rnd.Next(1, 18);
		_enemyTexture.Texture = GD.Load<Texture2D>($"res://assets/textures/enemies/enemy{enemyNumber}.png");
	}

	private void OnFightButtonPressed()
	{
		if (_player.Lives <= 0) return;

		int basePlayerDamage = Math.Max(1, _player.Attack - _enemy.Defense);
		bool isCrit = _rnd.Next(1, 101) <= _player.Luck;
		int playerDamage = isCrit ? basePlayerDamage * 2 : basePlayerDamage;

		int enemyDamage = Math.Max(1, _enemy.Attack - _player.Defense);

		_enemy.CurrentVitality -= playerDamage;
		_player.CurrentVitality -= enemyDamage;

		if (_enemy.CurrentVitality <= 0) WinRound();
		if (_player.CurrentVitality <= 0) LoseLife();
		
		RefreshUI();
	}

	private void WinRound()
	{
		int interestBonus = Math.Min(_player.Coins / 5, 5);
		_player.Coins += _currentRound + interestBonus;
		_currentRound++;
		GenerateNewEnemy();
		RefreshUI();
	}

	private void LoseLife()
	{
		_player.Lives--;
		_player.CurrentVitality = _player.MaxVitality;
		UpdateHeartsUI();
		
		if (_player.Lives <= 0)
		{
			// Načtení game_over scény
			GetTree().ChangeSceneToFile("res://scenes/game_over.tscn");
		}
	}

	private void RefreshUI()
	{
		_attackLabel.Text = $"Útok: {_player.Attack}";
		_defenseLabel.Text = $"Obrana: {_player.Defense}";
		_vitalityLabel.Text = $"Vitalita: {_player.Vitality}";
		_luckLabel.Text = $"Štěstí: {_player.Luck}%";
		_hpBar.MaxValue = _player.MaxVitality;
		_hpBar.Value = _player.CurrentVitality;
		_hpValueLabel.Text = $"{_player.CurrentVitality}/{_player.MaxVitality}";
		_coinsLabel.Text = $"Mince: {_player.Coins}";
		
		int currentInterest = Math.Min(_player.Coins / 5, 5);
		_interestLabel.Text = $"Úrok: +{currentInterest}";

		_enemyAttackLabel.Text = $"Útok: {_enemy.Attack}";
		_enemyDefenseLabel.Text = $"Obrana: {_enemy.Defense}";
		_enemyHpBar.MaxValue = _enemy.MaxVitality;
		_enemyHpBar.Value = _enemy.CurrentVitality;
		_enemyHpLabel.Text = $"{_enemy.CurrentVitality}/{_enemy.MaxVitality}";
	}

	private void UpdateHeartsUI()
	{
		Texture2D fullHeart = GD.Load<Texture2D>("res://assets/textures/other/HeartFull.png");
		Texture2D emptyHeart = GD.Load<Texture2D>("res://assets/textures/other/HeartEmpty.png");

		for (int i = 0; i < _hearts.Length; i++)
		{
			_hearts[i].Texture = i < _player.Lives ? fullHeart : emptyHeart;
		}
	}
}
