﻿using System.Linq;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using MoreLinq;
using UI.Interfaces;
using UI.Services;
using static System.Windows.Media.ColorConverter;

namespace UI.ViewModels
{
	public class FormsQueue : PropertyChangedBase, IViewModel
	{
	    private readonly ISubtractor subtractor;
	    private const int Limit = 5;
		public IFormFactory Factory { get; }
		public IObservableCollection<IForm> Forms { get; set; }

		public int RowIndex { get; } = 2;

		public bool CanAdd => Forms.Count < Limit;
		public bool CanRemove => Forms.Any();
		public bool CanSubmit => Forms.Any() && Forms.All(form => form.CanSubmit());

		public FormsQueue(IFormFactory factory, ISubtractor subtractor)
		{
		    this.subtractor = subtractor;
		    Forms = new BindableCollection<IForm>();
			Factory = factory;
		}

		public void Add()
		{
			var form = Factory.Create();
			form.PropertyChanged += (s, a) => NotifyOfPropertyChange(nameof(CanSubmit));
			
			Forms.Add(form);
            subtractor.Add(form);

            SetPrimaryColor();
			Refresh();
		}

		public void Remove()
		{
		    var last = Forms.Last();
		    Forms.Remove(last);
		    subtractor.Remove(last);

            Refresh();

			if (Forms.Any()) 
				SetPrimaryColor();
		}

		public void Submit()
		{
			Forms.ForEach(form => form.Submit());
			Forms.Clear();
            subtractor.Clear();


            Refresh();
		}

		private void SetPrimaryColor()
		{
			Forms.First().Background = Forms.Count > 1
				? (Brush) Application.Current?.FindResource("AccentColorBrush4")
				: Brushes.Transparent;
		}
	}
}