using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Caliburn.Micro;
using Common;
using Common.Storages;
using UI.Interfaces;
using static Common.Record;

namespace UI.ViewModels
{
	public class Form : PropertyChangedBase, IForm
	{
		private readonly IRecordsStorage aggregator;
		private readonly ISettings settings;
		private Types selectedType;
		private Brush background = Brushes.Transparent;
		private decimal amount;
		private string description;

		public Form(ISettings settings, IRecordsStorage aggregator)
		{
			this.settings = settings;
			this.aggregator = aggregator;

			Types = Enum.GetValues(typeof (Types)).Cast<Types>();
			UpdateCategories(selectedType);

			DateTime = DateTime.Now;
			Descriptions = settings.Descriptions;
		}

		public IEnumerable<Types> Types { get; set; }
		public IEnumerable<Categories> Categories { get; set; }

		public Types SelectedType
		{
			get { return selectedType; }
			set
			{
				selectedType = value;
				NotifyOfPropertyChange(nameof(SelectedType));
				UpdateCategories(selectedType);
			}
		}

		public Categories SelectedCategory { get; set; }

		public string Description
		{
			get { return description; }
			set
			{
				if (value == description) return;
				description = value;
				NotifyOfPropertyChange();
			}
		}

		public string[] Descriptions { get; set; }
		public DateTime DateTime { get; set; }

		public decimal Amount
		{
			get { return amount; }
			set
			{
				if (value == amount) return;
				amount = value;
				NotifyOfPropertyChange();
			}
		}

		public Brush Background
		{
			get { return background; }
			set
			{
				if (Equals(value, background)) return;
				background = value;
				NotifyOfPropertyChange();
			}
		}

		public void Submit()
		{
			aggregator.Add(new Record(Amount, SelectedType, SelectedCategory, Description, DateTime));
		}

		public bool CanSubmit()
		{
			if (Amount < 1) return false;
			if (string.IsNullOrWhiteSpace(Description)) return false;

			if (selectedType == Record.Types.Debt 
				&& Description != "In" 
				&& Description != "Out") return false;


			return true;
		}

		private void UpdateCategories(Types type)
		{
			Categories = settings.CategoriesMapping[type];
			SelectedCategory = Categories.First();

			NotifyOfPropertyChange(nameof(Categories));
			NotifyOfPropertyChange(nameof(SelectedCategory));
		}
	}
}