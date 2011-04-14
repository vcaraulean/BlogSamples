using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using RadTabControlAndCaliburn.ViewModels;
using Telerik.Windows.Controls;

namespace RadTabControlAndCaliburn
{
	public class Bootstrapper : Bootstrapper<MainPageViewModel>
	{
		private IWindsorContainer container;

		protected override void Configure()
		{
			container = new WindsorContainer();
			container.Register(
				Component.For<MainPageViewModel>(),
				Component.For<FirstTabItemViewModel>(),
				Component.For<SecondTabItemViewModel>()
				);

			ConventionManager
				.AddElementConvention<RadTabControl>(RadTabControl.ItemsSourceProperty,
			                                                      "ItemsSource",
			                                                      "SelectionChanged")
				.ApplyBinding = (viewModelType, path, property, element, convention) =>
				{
					if (!ConventionManager.SetBinding(viewModelType, path, property, element, convention))
						return false;

					var tabControl = (RadTabControl)element;
					if (tabControl.ContentTemplate == null
						&& tabControl.ContentTemplateSelector == null
						&& property.PropertyType.IsGenericType)
					{
						var itemType = property.PropertyType.GetGenericArguments().First();
						if (!itemType.IsValueType && !typeof(string).IsAssignableFrom(itemType))
							tabControl.ContentTemplate = ConventionManager.DefaultItemTemplate;
					}
					ConventionManager.ConfigureSelectedItem(element,
															RadTabControl.SelectedItemProperty,
															viewModelType,
															path);

					if (string.IsNullOrEmpty(tabControl.DisplayMemberPath))
						ConventionManager.ApplyHeaderTemplate(tabControl,
															  RadTabControl.ItemTemplateProperty,
															  viewModelType);
					return true;
				};
		}

		protected override object GetInstance(Type service, string key)
		{
			if (string.IsNullOrWhiteSpace(key))
				return container.Resolve(service);

			return container.Resolve(key, service);
		}

		protected override IEnumerable<object> GetAllInstances(Type service)
		{
			return container.ResolveAll(service).Cast<object>();
		}

		protected override void BuildUp(object instance)
		{
			CollectionExtensions.ForEach(instance.GetType().GetProperties()
			                             	.Where(property => property.CanWrite && property.PropertyType.IsPublic)
			                             	.Where(property => container.Kernel.HasComponent(property.PropertyType)),
			                             property => property.SetValue(instance, container.Resolve(property.PropertyType), null));
		}
	}
}