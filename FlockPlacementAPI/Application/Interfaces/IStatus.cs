using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
	public interface IStatus
	{
		bool IsStatusReady(Guid id);
		bool FindStatusItem(Guid id);
		Guid WriteStatusItem(Guid id);
		void UpdateStatusItem(Guid id);
	}
}
