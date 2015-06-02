using System.ComponentModel;
using System.Runtime.CompilerServices;
using Statistics.Banking;
using Unity = Microsoft.Practices.Unity;

namespace Statistics.ViewModels
{
	public class Funds : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		private int cards;
		private int debts;
		private int cash;
		private int upwork;

		public int Upwork
		{
			get { return upwork; }
			set { upwork = value; OnPropertyChanged(); }
		}

		public int Cards
		{
			get { return cards; }
			set { cards = value; OnPropertyChanged(); }
		}

		public int Cash
		{
			get { return cash; }
			set { cash = value; OnPropertyChanged(); }
		}

		public int Debts
		{
			get { return debts; }
			set { debts = value; OnPropertyChanged(); }
		}

		public int Total { get; set; }

		public Funds([Unity.Dependency("bank")] IFundsStorage bank, [Unity.Dependency("debt")]IFundsStorage debt)
		{
			PropertyChanged += UpdateTotal;

			bank.Get(amount => Cards = (int) amount);
			debt.Get(amount => Debts = (int) amount);
		}

		private void UpdateTotal(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
		{
			Total = Upwork + Cards + Cash + Debts;
		}

		public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}