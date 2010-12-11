using Caliburn.Micro;
using RadTabControlAndCaliburn.ViewModels;

namespace RadTabControlAndCaliburn
{
	public class MainPageViewModel : Conductor<IScreen>.Collection.OneActive
	{
		private readonly FirstTabItemViewModel first;
		private readonly SecondTabItemViewModel second;

		public MainPageViewModel(FirstTabItemViewModel first, SecondTabItemViewModel second)
		{
			this.first = first;
			this.second = second;
		}

		protected override void OnInitialize()
		{
			base.OnInitialize();
			Items.Add(first);
			Items.Add(second);

			ActivateItem(first);
		}
	}
}