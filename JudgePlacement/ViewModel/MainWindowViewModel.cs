using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JudgePlacement.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        public ApplicationControlViewModel ApplicationControl { get; set; } = new();

        public TournamentControlViewModel TournamentControl { get; set; } = new();

        public MainWindowViewModel() { }
    }
}
