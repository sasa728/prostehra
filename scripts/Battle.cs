using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Battle : Control
{
	// 1. PLAYER AND ENEMY DATA
	public class PlayerStats
	{
		public int BaseAttack = 15;
		public int BaseDefense = 8;
		public int BaseVitality = 15;
		public int BaseLuck = 3;
		public int CurrentVitality;
		public int Coins = 10;
		public int Lives = 3;

		public int MaxVitality => BaseVitality * 10;
		public int Attack { get; set; }
		public int Defense { get; set; }
		public int Vitality => BaseVitality;
		public int Luck { get; set; }

		public PlayerStats()
		{
			CurrentVitality = MaxVitality;
			Attack = BaseAttack;
			Defense = BaseDefense;
			Luck = BaseLuck;
		}
	}

	public class EnemyStats
	{
		public int Attack;
		public int Defense;
		public int MaxVitality;
		public int CurrentVitality;
	}

	// 2. SHOP AND INVENTORY DATA
	public enum ShopItemType
	{
		AttackPotion,
		DefensePotion,
		HealthPotion,
		LuckPotion,
		Sword,
		Shield,
		Food,
		Shamrock
	}

	public class ShopItem
	{
		public ShopItemType Type;
		public string TexturePath;
		public int Price;
	}

	private PlayerStats _player = new PlayerStats();
	private EnemyStats _enemy = new EnemyStats();
	private int _currentRound = 1;
	private Random _rnd = new Random();
	private List<int> _enemyPool = new List<int>();
	
	private List<ShopItem> _currentShopItems = new List<ShopItem>();
	private List<ShopItem> _inventory = new List<ShopItem>();
	private int _tempAttackBoost;
	private int _tempDefenseBoost;
	private int _tempLuckBoost;

	// 3. UI REFERENCES
	private Label _attackLabel, _defenseLabel, _vitalityLabel, _luckLabel;
	private ProgressBar _hpBar, _enemyHpBar;
	private Label _hpValueLabel, _enemyHpLabel;
	private Label _coinsLabel, _interestLabel;
	private Label _enemyAttackLabel, _enemyDefenseLabel, _enemyVitalityLabel;
	private TextureRect _enemyTexture;
	private Button _fightButton;
	private TextureRect[] _hearts;
	private Label _roundLabel;
	private TextureRect[] _interestCoins;
	
	// Shop and inventory
	private TextureRect[] _shopSlots = new TextureRect[4];
	private Label[] _shopPriceLabels = new Label[4];
	private TextureRect[] _inventorySlots = new TextureRect[4];
	private Button _rerollButton;

	public override void _Ready()
	{
		InitializeEnemyPool();
		InitializeUIReferences();
		GenerateNewEnemy();
		InitializeShop();
		RefreshUI();

		_fightButton = GetNode<Button>("EnemyContainer/FightButton");
		_fightButton.Pressed += OnFightButtonPressed;

		_hearts = new TextureRect[3]
		{
			GetNode<TextureRect>("LivesPanel/Live1"),
			GetNode<TextureRect>("LivesPanel/Live2"),
			GetNode<TextureRect>("LivesPanel/Live3")
		};
		
		_interestCoins = new TextureRect[5]
		{
			GetNode<TextureRect>("InterestPanel/Coin1"),
			GetNode<TextureRect>("InterestPanel/Coin2"),
			GetNode<TextureRect>("InterestPanel/Coin3"),
			GetNode<TextureRect>("InterestPanel/Coin4"),
			GetNode<TextureRect>("InterestPanel/Coin5")
		};
		
		UpdateHeartsUI();
	}

	private void InitializeEnemyPool()
	{
		var enemies = Enumerable.Range(1, 17).ToList();
		
		// Shuffle enemies 1-17
		for (int i = enemies.Count - 1; i > 0; i--)
		{
			int j = _rnd.Next(i + 1);
			(enemies[i], enemies[j]) = (enemies[j], enemies[i]);
		}
		
		// Add final enemy at position 18
		enemies.Add(18);
		_enemyPool = enemies;
	}

	private void InitializeUIReferences()
	{
		// Player
		_attackLabel = GetNode<Label>("PlayerPanel/PlayerData/PlayerStats/PlayerAttack");
		_defenseLabel = GetNode<Label>("PlayerPanel/PlayerData/PlayerStats/PlayerDefense");
		_vitalityLabel = GetNode<Label>("PlayerPanel/PlayerData/PlayerStats/PlayerVitality");
		_luckLabel = GetNode<Label>("PlayerPanel/PlayerData/PlayerStats/PlayerLuck");
		_hpBar = GetNode<ProgressBar>("PlayerPanel/PlayerData/PlayerStats/ProgressBar");
		_hpValueLabel = GetNode<Label>("PlayerPanel/PlayerData/PlayerStats/ProgressBar/HpLabel");
		_coinsLabel = GetNode<Label>("PlayerPanel/PlayerData/PlayerMoney/PlayerCoins");
		_interestLabel = GetNode<Label>("PlayerPanel/PlayerData/PlayerMoney/PlayerInterrest");
		_roundLabel = GetNode<Label>("RoundLabel");

		// Enemy
		_enemyAttackLabel = GetNode<Label>("PlayerPanel/PlayerData/EnemyStats/EnemyAttack");
		_enemyDefenseLabel = GetNode<Label>("PlayerPanel/PlayerData/EnemyStats/EnemyDefense");
		_enemyVitalityLabel = GetNode<Label>("PlayerPanel/PlayerData/EnemyStats/EnemyVitality");
		_enemyHpBar = GetNode<ProgressBar>("EnemyContainer/ProgressBar");
		_enemyHpLabel = GetNode<Label>("EnemyContainer/ProgressBar/HpLabel");
		_enemyTexture = GetNode<TextureRect>("EnemyContainer/Enemy");
	}

	private void InitializeShop()
	{
		_rerollButton = GetNode<Button>("ShopPanel/RerollButton");
		_rerollButton.Pressed += OnRerollPressed;

		for (int i = 0; i < 4; i++)
		{
			_shopSlots[i] = GetNode<TextureRect>($"ShopPanel/ItemSlot{i+1}");
			_shopPriceLabels[i] = GetNode<Label>($"ShopPanel/ItemSlot{i+1}/ItemSlot{i+1}Label");
			int index = i;
			_shopSlots[i].GuiInput += (e) => OnShopItemClicked(e, index);
		}

		for (int i = 0; i < 4; i++)
		{
			_inventorySlots[i] = GetNode<TextureRect>($"IventoryPanel/InventorySlot{i+1}");
			int index = i;
			_inventorySlots[i].GuiInput += (e) => OnInventoryItemClicked(e, index);
		}

		GenerateShopItems();
		UpdateShopUI();
	}

	private void GenerateShopItems()
	{
		_currentShopItems.Clear();
		for (int i = 0; i < 4; i++)
		{
			var item = new ShopItem
			{
				Type = (ShopItemType)_rnd.Next(0, 8),
				Price = _rnd.Next(2, 5)
			};
			item.TexturePath = GetTexturePathForItem(item.Type);
			_currentShopItems.Add(item);
		}
	}

	private string GetTexturePathForItem(ShopItemType type)
	{
		return type switch
		{
			ShopItemType.AttackPotion => "res://assets/textures/shop/AttackPotion.png",
			ShopItemType.DefensePotion => "res://assets/textures/shop/DefensePotion.png",
			ShopItemType.HealthPotion => "res://assets/textures/shop/HealthPotion.png",
			ShopItemType.LuckPotion => "res://assets/textures/shop/LuckPotion.png",
			ShopItemType.Sword => "res://assets/textures/shop/sword.png",
			ShopItemType.Shield => "res://assets/textures/shop/shield.png",
			ShopItemType.Food => "res://assets/textures/shop/food.png",
			ShopItemType.Shamrock => "res://assets/textures/shop/shamrock.png",
			_ => ""
		};
	}

	private void OnShopItemClicked(InputEvent @event, int slotIndex)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			if (slotIndex >= _currentShopItems.Count) return;

			var item = _currentShopItems[slotIndex];
			if (_player.Coins >= item.Price)
			{
				_player.Coins -= item.Price;
				
				if (IsPermanentItem(item.Type))
				{
					ApplyImmediateEffect(item);
				}
				else
				{
					_inventory.Add(item);
				}
				
				_currentShopItems.RemoveAt(slotIndex);
				UpdateShopUI();
				UpdateInventoryUI();
				RefreshUI();
			}
		}
	}

	private bool IsPermanentItem(ShopItemType type)
	{
		return type switch
		{
			ShopItemType.Sword or ShopItemType.Shield 
				or ShopItemType.Food or ShopItemType.Shamrock => true,
			_ => false
		};
	}

	private void ApplyImmediateEffect(ShopItem item)
	{
		switch (item.Type)
		{
			case ShopItemType.Sword:
				_player.BaseAttack += 3;
				_player.Attack = _player.BaseAttack + _tempAttackBoost;
				break;
			case ShopItemType.Shield:
				_player.BaseDefense += 3;
				_player.Defense = _player.BaseDefense + _tempDefenseBoost;
				break;
			case ShopItemType.Food:
				_player.BaseVitality += 2;
				int healAmount = (int)(_player.MaxVitality * 0.5f);
				_player.CurrentVitality = Math.Min(_player.CurrentVitality + healAmount, _player.MaxVitality);
				break;
			case ShopItemType.Shamrock:
				_player.BaseLuck += 2;
				_player.Luck = _player.BaseLuck + _tempLuckBoost;
				break;
		}
	}

	private void OnRerollPressed()
	{
		if (_player.Coins >= 1)
		{
			_player.Coins--;
			GenerateShopItems();
			UpdateShopUI();
			RefreshUI();
		}
	}

	private void OnInventoryItemClicked(InputEvent @event, int slotIndex)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			if (slotIndex >= _inventory.Count) return;

			var item = _inventory[slotIndex];
			UseItem(item);
			_inventory.RemoveAt(slotIndex);
			UpdateInventoryUI();
			RefreshUI();
		}
	}

	private void UseItem(ShopItem item)
	{
		switch (item.Type)
		{
			case ShopItemType.AttackPotion:
				_tempAttackBoost += 2;
				_player.Attack = _player.BaseAttack + _tempAttackBoost;
				break;
			case ShopItemType.DefensePotion:
				_tempDefenseBoost += 2;
				_player.Defense = _player.BaseDefense + _tempDefenseBoost;
				break;
			case ShopItemType.HealthPotion:
				_player.CurrentVitality = Math.Min(
					_player.CurrentVitality + 50,
					_player.MaxVitality
				);
				break;
			case ShopItemType.LuckPotion:
				_tempLuckBoost += 10;
				_player.Luck = _player.BaseLuck + _tempLuckBoost;
				break;
		}
	}

	private void UpdateShopUI()
	{
		for (int i = 0; i < 4; i++)
		{
			if (i < _currentShopItems.Count)
			{
				_shopSlots[i].Texture = GD.Load<Texture2D>(_currentShopItems[i].TexturePath);
				_shopPriceLabels[i].Text = _currentShopItems[i].Price.ToString();
			}
			else
			{
				_shopSlots[i].Texture = null;
				_shopPriceLabels[i].Text = "";
			}
		}
	}

	private void UpdateInventoryUI()
	{
		for (int i = 0; i < 4; i++)
		{
			_inventorySlots[i].Texture = i < _inventory.Count 
				? GD.Load<Texture2D>(_inventory[i].TexturePath)
				: null;
		}
	}

	private void GenerateNewEnemy()
	{
		if (_currentRound > 18) return;

		int enemyIndex = _currentRound - 1;
		int enemyNumber = _enemyPool[enemyIndex];

		_enemy.Attack = 8 + _currentRound * 2;
		_enemy.Defense = 6 + _currentRound * 1;
		_enemy.MaxVitality = 60 + _currentRound * 10;
		_enemy.CurrentVitality = _enemy.MaxVitality;

		_enemyTexture.Texture = GD.Load<Texture2D>($"res://assets/textures/enemies/enemy{enemyNumber}.png");
	}

	private void OnFightButtonPressed()
	{
		_player.Attack = _player.BaseAttack + _tempAttackBoost;
		_player.Defense = _player.BaseDefense + _tempDefenseBoost;
		_player.Luck = _player.BaseLuck + _tempLuckBoost;

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
		// Reset temp bonuses when round increases
		_tempAttackBoost = 0;
		_tempDefenseBoost = 0;
		_tempLuckBoost = 0;
		_player.Attack = _player.BaseAttack;
		_player.Defense = _player.BaseDefense;
		_player.Luck = _player.BaseLuck;

		int interestBonus = Math.Min(_player.Coins / 5, 5);
		_player.Coins += _currentRound * 2 + interestBonus;
		_currentRound++;
		
		if(_currentRound > 18)
		{
			GetTree().ChangeSceneToFile("res://scenes/game_won.tscn");
			return;
		}
		
		GenerateNewEnemy();
		GenerateShopItems();
		UpdateShopUI();
		RefreshUI();
	}

	private void LoseLife()
	{
		_player.Lives--;
		_player.CurrentVitality = _player.MaxVitality;
		UpdateHeartsUI();
		
		if (_player.Lives <= 0)
		{
			GetTree().ChangeSceneToFile("res://scenes/game_over.tscn");
		}
	}

	private void RefreshUI()
	{
		_roundLabel.Text = $"Round: {_currentRound}";
		
		// Player stats
		_attackLabel.Text = $"Attack: {_player.BaseAttack} (+{_tempAttackBoost})";
		_defenseLabel.Text = $"Defense: {_player.BaseDefense} (+{_tempDefenseBoost})";
		_vitalityLabel.Text = $"Vitality: {_player.Vitality}";
		_luckLabel.Text = $"Luck: {_player.BaseLuck} (+{_tempLuckBoost})%";
		_hpBar.MaxValue = _player.MaxVitality;
		_hpBar.Value = _player.CurrentVitality;
		_hpValueLabel.Text = $"{_player.CurrentVitality}/{_player.MaxVitality}";
		_coinsLabel.Text = $"Coins: {_player.Coins}";
		
		// Enemy stats
		_enemyAttackLabel.Text = $"Attack: {_enemy.Attack}";
		_enemyDefenseLabel.Text = $"Defense: {_enemy.Defense}";
		_enemyVitalityLabel.Text = $"Vitality: {_enemy.MaxVitality / 10}";
		_enemyHpBar.MaxValue = _enemy.MaxVitality;
		_enemyHpBar.Value = _enemy.CurrentVitality;
		_enemyHpLabel.Text = $"{_enemy.CurrentVitality}/{_enemy.MaxVitality}";
		
		int currentInterest = Math.Min(_player.Coins / 5, 5);
		_interestLabel.Text = $"Interest: +{currentInterest}";
		UpdateInterestCoinsUI(currentInterest);
		
		UpdateShopUI();
		UpdateInventoryUI();
	}

	private void UpdateInterestCoinsUI(int currentInterest)
	{
		if (_interestCoins == null) return;

		for (int i = 0; i < _interestCoins.Length; i++)
		{
			if (_interestCoins[i] != null)
			{
				_interestCoins[i].Visible = (i + 1) <= currentInterest;
			}
		}
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
