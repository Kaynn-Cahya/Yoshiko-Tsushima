using Discord.Commands;
using System;
using System.Data;
using System.Threading.Tasks;
using Discord;

namespace YoshikoBot.CommandModules {
    public class BasicModule : ModuleBase<SocketCommandContext> {
		[Command("Math"), Alias("calculate", "eval", "cal")]
		[Summary("Calculates a given expression.")]
		public Task MathAsync([Remainder] string expression) {

			string result;
			try {
				result = new DataTable().Compute(expression, null).ToString();
			} catch (EvaluateException) {
				result = "Invalid expression to calculate.";
			} catch (OverflowException) {
				// TODO: Happens when large calcuation (multiplication) is done in Int32.
				// Simple solution: Trick it into calculating in decimals or a larger data number.
				result = "Number is too complicated for me.";
			} catch (Exception e) {
				result = "Invalid expression to calculate.";
				Logger.Log(LogSeverity.Error, $"Unusual {e.GetBaseException().GetType().Name} at MathAsync Command in BasicModule; ({e.Message}); @ {e.Source}");
			}

		    return ReplyAsync(result);
		}
	}
}
