using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wendy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wendy.Tests
{
    [TestClass()]
    public class WinCCConvertorTests
    {
        [TestMethod()]
        public void ValidateTest()
        {
            Assert.IsTrue(WinCCConvertor.Validate(""));
            Assert.IsTrue(WinCCConvertor.Validate("'KL_FUEK_VT_234_ZLA_SCH1___bm_bm_bit_86'"));
            Assert.IsTrue(WinCCConvertor.Validate("KL_FUEK_VT_234_ZLA_SCH1___bm_bm_bit_86"));
            Assert.IsTrue(WinCCConvertor.Validate("'KL_EHBU_FT_204_PB1_2___bm_bm_bit_359' and 'KL_EHBU_FT_204_PB1_2___bm_bm_bit_298'"));
            Assert.IsTrue(WinCCConvertor.Validate("'KL_KOMP_VT_RMK1_SS00___di_istwert_9'"));
            Assert.IsTrue(WinCCConvertor.Validate("M3A_RLT1_SK03_SS00___pr_stoer_offen"));
            Assert.IsTrue(WinCCConvertor.Validate("KL_KOMP_VT_RMK1_SS00___pr_still_prio_1"));
            Assert.IsTrue(WinCCConvertor.Validate("'diagnostika' && ('206_VW12_pozice' || '206_VW12_cil')"));
            Assert.IsTrue(WinCCConvertor.Validate("'KL_KTLT_VT_231_SS00___bm_bm_bit_395' || 'KL_KTLT_VT_231_SS00___bm_bm_bit_396'  || 'KL_KTLT_VT_231_SS00___bm_bm_bit_397' || 'KL_KTLT_VT_231_SS00___bm_bm_bit_398' "));


            Assert.IsFalse(WinCCConvertor.Validate("'KL_EHBU_FT_204_PB1_2___bm_bm_bit_359and"));
            //Assert.IsFalse(WinCCConvertor.Validate("KL_EHBU_FT_204_PB1_2___mm_bm_bit_359"));
            Assert.IsFalse(WinCCConvertor.Validate("KL_EHBU_FT_204_PB1_2___bm_bm_bit359"));
            Assert.IsFalse(WinCCConvertor.Validate("EHBU_FT_204_PB1_2___bm_bm_bit359"));
        }

        [TestMethod()]
        public void TranslateTest()
        {
            if (!WinCCConvertor.Translate("").Equals(""))
                Assert.Fail();

            var result = WinCCConvertor.Translate("'KL_FUEK_VT_234_ZLA_SCH1___bm_bm_bit_86'");
            var expected = @"\\ELAK\VT.234.ZLA.SCH1.BM.BM.086";
            if (!result.Equals(expected))
                Assert.Fail("Bad result {0} - {1}", result, expected);


            result = WinCCConvertor.Translate("KL_FUEK_VT_234_SS00___di_obau_6");
            expected = @"\\ELAK\VT.234.SS00.DI.OA.006";
            if (!result.Equals(expected))
                Assert.Fail("Bad result {0} - {1}", result, expected);

            result = WinCCConvertor.Translate("'KL_EHBU_FT_204_PB1_2___bm_bm_bit_359' and 'KL_EHBU_FT_204_PB1_2___bm_bm_bit_298'");
            expected = @"\\ELAK\FT.204.PB1.2.BM.BM.359 and \\ELAK\FT.204.PB1.2.BM.BM.298";
            if (!result.Equals(expected))
                Assert.Fail("Bad result {0} - {1}", result, expected);

            result = WinCCConvertor.Translate("KL_EHBU_FT_204_PB1_2___bm_bm_bit_359 and KL_EHBU_FT_204_PB1_2___bm_bm_bit_298");
            expected = @"\\ELAK\FT.204.PB1.2.BM.BM.359 and \\ELAK\FT.204.PB1.2.BM.BM.298";
            if (!result.Equals(expected))
                Assert.Fail("Bad result {0} - {1}", result, expected);

            result = WinCCConvertor.Translate("'KL_KOMP_VT_RMK1_SS00___di_istwert_9'");
            expected = @"\\ELAK\VT.RMK1.SS00.DI.IW.009";
            if (!result.Equals(expected))
                Assert.Fail("Bad result {0} - {1}", result, expected);

            result = WinCCConvertor.Translate("KL_RBAP_FT_203_BP1_0001___pr_stoer_offen");
            expected = @"\\ELAK\FT.203.BP1.0001.STOER.OFFEN";
            if (!result.Equals(expected))
                Assert.Fail("Bad result {0} - {1}", result, expected);

            result = WinCCConvertor.Translate("'diagnostika' && ('206_VW12_pozice' || '206_VW12_cil')");
            expected = @"\\ELAK\diagnostika and (\\ELAK\206_VW12_pozice or \\ELAK\206_VW12_cil)";
            if (!result.Equals(expected))
                Assert.Fail("Bad result {0} - {1}", result, expected);
        }
    }
}