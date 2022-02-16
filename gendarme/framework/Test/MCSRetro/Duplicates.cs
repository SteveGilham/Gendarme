
namespace Retro
{
    public class NonDuplicatedCodeIntoForeachLoop
    {
    private static System.Collections.IList myList;

      public void PrintValues()
      {
        foreach (string value in myList)
          System.Console.WriteLine(value);
      }

      public void PrintValuesInSameLine()
      {
        foreach (string value in myList)
          System.Console.Write(value);
      }
    }

    public class NonDuplicatedInSwitchLoadingByFields
    {
      private string option = "LOW";
      private Severity severity = Severity.Low;
      private Confidence confidence = Confidence.Normal;

      //Althoug both are referring to option as argument, it
      //isn't the same switch.
      public void FirstNonDuplicatedSwitch()
      {
        switch (option)
        {
          case "AUDIT":
          case "AUDIT+":
          case "AUDIT-":
            severity = Severity.Audit;
            break;

          case "LOW":
          case "LOW+":
          case "LOW-":
            severity = Severity.Low;
            break;

          case "MEDIUM":
          case "MEDIUM+":
          case "MEDIUM-":
            severity = Severity.Medium;
            break;

          case "HIGH":
          case "HIGH+":
          case "HIGH-":
            severity = Severity.High;
            break;

          case "CRITICAL":
          case "CRITICAL+":
          case "CRITICAL-":
            severity = Severity.Critical;
            break;

          default:
            break;
        }
      }

      public void SecondNonDuplicatedSwitch()
      {
        switch (option)
        {
          case "LOW":
          case "LOW+":
          case "LOW-":
            confidence = Confidence.Low;
            break;

          case "NORMAL":
          case "NORMAL+":
          case "NORMAL-":
            confidence = Confidence.Normal;
            break;

          case "HIGH":
          case "HIGH+":
          case "HIGH-":
            confidence = Confidence.High;
            break;

          case "TOTAL":
          case "TOTAL+":
          case "TOTAL-":
            confidence = Confidence.Total;
            break;

          default:
            break;
        }
      }
    }
    
	public enum Severity {
		/// <summary>
		/// The code can not work as expected.
		/// </summary>
		Critical,
		/// <summary>
		/// The code may work or fails depending on values, configuration...
		/// </summary>
		High,
		/// <summary>
		/// The code will work most of the time or on the default, or most, common configuration
		/// </summary>
		Medium,
		/// <summary>
		/// The actual code works, fixing the defect doesn't have a big impact.
		/// By default some runners won't display such low severity issues to keep the number of defects to a reasonable level.
		/// </summary>
		Low,
		/// <summary>
		/// The actual code works but should be reviewed for potential problems.
		/// Often the code cannot be changed to satisfy the rule logic, 
		/// i.e. the rule will always report it unless the rule or defect is ignored.
		/// </summary>
		Audit
	}

        	public enum Confidence {
		/// <summary>
		/// The rule is 100% certain of its result. 
		/// There should never be false positives for Total.
		/// </summary>
		Total,
		/// <summary>
		/// The rule is near 100% certain of its result.
		/// A few false-positives are possible.
		/// </summary>
		High,
		/// <summary>
		/// The rule has found a potential defect but cannot be certain of the result.
		/// Some false positive are to be expected in the results.
		/// </summary>
		Normal,
		/// <summary>
		/// The rule doesn't have enough information to be certain about the defect.
		/// Many of the results are likely to be false positives. 
		/// By default some runners wont display results if the confidence on the defect is low.
		/// </summary>
		Low
	}

} // namespace
