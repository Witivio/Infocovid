using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Witivio.Sdk;

namespace InfosCovid19.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiagnosticController : ControllerBase
    {
        private const string ValeurOui = "oui";

        [HttpPost()]
        public IActionResult Post([FromBody]VisioDialog dialog)
        {
            var q1 = dialog.GetPropertyValue("Q1");
            var q2 = dialog.GetPropertyValue("Q2");
            var q3 = dialog.GetPropertyValue("Q3");
            var q4 = dialog.GetPropertyValue("Q4");
            var q5 = dialog.GetPropertyValue("Q5");
            var q6 = dialog.GetPropertyValue("Q6");
            var q6Bis = dialog.GetPropertyValue("Q6bis");
            var q7 = dialog.GetPropertyValue("Q7");
            var q8 = dialog.GetPropertyValue("Q8");
            var q9 = dialog.GetPropertyValue("Q9");
            var q10 = dialog.GetPropertyValue("Q10");
            var q11 = dialog.GetPropertyValue("Q11");
            var q12 = dialog.GetPropertyValue("Q12");
            var q13 = dialog.GetPropertyValue("Q13");
            var q14 = dialog.GetPropertyValue("Q14");
            var q15 = dialog.GetPropertyValue("Q15");
            var q16 = dialog.GetPropertyValue("Q16");
            var q17 = dialog.GetPropertyValue("Q17");
            var q18 = dialog.GetPropertyValue("Q18");
            var q19 = dialog.GetPropertyValue("Q19");
            var q20 = dialog.GetPropertyValue("Q20");
            var q21 = dialog.GetPropertyValue("Q21");

            bool aDeLaFievre = (q1.Contains("37,8", StringComparison.InvariantCultureIgnoreCase) || q1.Contains("39", StringComparison.InvariantCultureIgnoreCase));
            bool aDeLaToux = q2.Contains(ValeurOui, StringComparison.InvariantCultureIgnoreCase);
            bool aDeLAnosmie = q3.Contains(ValeurOui, StringComparison.InvariantCultureIgnoreCase);
            bool aDesDouleurs = q4.Contains(ValeurOui, StringComparison.InvariantCultureIgnoreCase);
            bool aDeLaDiarrhé = q5.Contains(ValeurOui, StringComparison.InvariantCultureIgnoreCase);

            var poids = int.Parse(q10);

            var taille = double.Parse(q11);
            if (taille >= 100)
                taille = taille / 100;

            var imc = poids / (taille * taille);

            var facteursPronostique = new List<bool>();
            if (q9.Contains("70", StringComparison.InvariantCultureIgnoreCase)) facteursPronostique.Add(true);
            if (imc >= 30) facteursPronostique.Add(true);
            if (q12.Contains(ValeurOui, StringComparison.InvariantCultureIgnoreCase)) facteursPronostique.Add(true);
            if (q13.Contains(ValeurOui, StringComparison.InvariantCultureIgnoreCase)) facteursPronostique.Add(true);
            if (q14.Contains(ValeurOui, StringComparison.InvariantCultureIgnoreCase)) facteursPronostique.Add(true);
            if (q15.Contains(ValeurOui, StringComparison.InvariantCultureIgnoreCase)) facteursPronostique.Add(true);
            if (q16.Contains(ValeurOui, StringComparison.InvariantCultureIgnoreCase)) facteursPronostique.Add(true);
            if (q17.Contains(ValeurOui, StringComparison.InvariantCultureIgnoreCase)) facteursPronostique.Add(true);
            if (q18.Contains(ValeurOui, StringComparison.InvariantCultureIgnoreCase)) facteursPronostique.Add(true);
            if (q19.Contains(ValeurOui, StringComparison.InvariantCultureIgnoreCase)) facteursPronostique.Add(true);
            if (q20.Contains(ValeurOui, StringComparison.InvariantCultureIgnoreCase)) facteursPronostique.Add(true);

            var facteursMajeurs = new List<bool>();
            if (q7.Contains(ValeurOui, StringComparison.InvariantCultureIgnoreCase)) facteursMajeurs.Add(true);
            if (q8.Contains(ValeurOui, StringComparison.InvariantCultureIgnoreCase)) facteursMajeurs.Add(true);

            var facteursMineurs = new List<bool>();
            if (q1.Contains("35,5", StringComparison.InvariantCultureIgnoreCase) || q1.Contains("39", StringComparison.InvariantCultureIgnoreCase) || q1.Contains("pas", StringComparison.InvariantCultureIgnoreCase))
                facteursMineurs.Add(true);
            if (q6Bis != null)
                facteursMineurs.Add(q6Bis.Contains(ValeurOui, StringComparison.InvariantCultureIgnoreCase));

            if (q9.Contains("moins", StringComparison.InvariantCultureIgnoreCase))
            {
                return Ok(new { end = "FIN1" });
            }

            if (facteursMajeurs.Count() >= 1)
            {
                return Ok(new { end = "FIN5" });
            }

            if (aDeLaFievre && aDeLaToux)
            {
                if (facteursPronostique.Count() == 0)
                    return Ok(new { end = "FIN6" });
                else
                {
                    if (facteursMineurs.Count() == 0 || facteursMineurs.Count() == 1)
                    {
                        return Ok(new { end = "FIN6" });
                    }
                    else
                    {
                        return Ok(new { end = "FIN4" });
                    }
                }
            }

            if (aDeLaFievre ||
                    (
                        aDeLaDiarrhé || (aDeLaToux && aDesDouleurs) || (aDeLaToux && aDeLAnosmie) || (aDesDouleurs && aDeLAnosmie)
                    )
                )
            {
                if (facteursPronostique.Count() == 0)
                {
                    if (facteursMineurs.Count() == 0)
                    {
                        if (q9.Contains("< 50", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return Ok(new { end = "FIN2" });
                        }
                        else
                        {
                            return Ok(new { end = "FIN3" });
                        }
                    }
                    if (facteursMineurs.Count() >= 1)
                    {
                        return Ok(new { end = "FIN3" });
                    }
                }
                if (facteursPronostique.Count() >= 1)
                {
                    if (facteursMineurs.Count() == 0 || facteursMineurs.Count() == 1)
                    {
                        return Ok(new { end = "FIN3" });
                    }
                    else
                    {
                        return Ok(new { end = "FIN4" });
                    }
                }
            }

            if (aDeLaToux ^ aDesDouleurs ^ aDeLAnosmie)
            {
                if (facteursPronostique.Count() == 0)
                {
                    return Ok(new { end = "FIN2" });
                }
                else
                {
                    return Ok(new { end = "FIN7" });
                }
            }
            else
            {
                return Ok(new { end = "FIN8" });
            }

        }
    }
}
