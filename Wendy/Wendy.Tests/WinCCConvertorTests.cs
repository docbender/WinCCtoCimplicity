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
            Assert.IsTrue(WinCCConvertor.Validate("('UserAdmin' || 'UserDisp') && (('207_EP09_F4_test_PIN' == 0x3838) || ('207_EP09_F4_test_typ' == 0x30303030) || ('207_EP09_F4_test_typ' == 0x20202020) || ('207_EP09_F4_test_barva' == 0x30303030) || ('207_EP09_F4_test_barva' == 0x20202020))"));

            Assert.IsFalse(WinCCConvertor.Validate("'KL_EHBU_FT_204_PB1_2___bm_bm_bit_359and"));            
            Assert.IsFalse(WinCCConvertor.Validate("KL_EHBU_FT_204_PB1_2___bm_bm_bit359"));
            Assert.IsFalse(WinCCConvertor.Validate("EHBU_FT_204_PB1_2___bm_bm_bit359"));

            Assert.IsTrue(WinCCConvertor.Validate("'KL_BRK2_FT_350_BP13___bm_bm_bit_228'+'KL_BRK2_FT_350_SS00___bm_bm_bit_67'+'KL_DLLZ_FT_218_SS00___bm_bm_bit_93'+'KL_DLLZ_FT_218_SS00___bm_bm_bit_92'+'KL_DLLZ_FT_218_SS00___bm_bm_bit_91'+'KL_DLLZ_FT_218_SS00___bm_bm_bit_90'+'KL_DLLZ_FT_217_SS00___bm_bm_bit_83'+'KL_DLLZ_FT_217_SS00___bm_bm_bit_87'+'KL_HKEA_FT_215_SS00___bm_bm_bit_87'+'KL_HKEA_FT_215_SS00___bm_bm_bit_83'+'KL_REP-_FT_214_SS00___bm_bm_bit_84'+'KL_FSD-_FT_213_SS00___bm_bm_bit_148'+'KL_FSD-_FT_213_SS00___bm_bm_bit_84'+'KL_DL--_FT_211_SS00___bm_bm_bit_84'+'KL_FUES_FT_209_SS00___bm_bm_bit_84'+'KL_FUES_FT_209_SS00___bm_bm_bit_92'+'KL_FUEK_FT_208_SS00___bm_bm_bit_86'+'KL_FUEK_FT_208_SS00___bm_bm_bit_91'+'KL_KTLS_FT_207_SS00___bm_bm_bit_92'+'KL_UBST_FT_206_SS00___bm_bm_bit_84'+'KL_SKRF_FT_205_SS00___bm_bm_bit_84'+'KL_BRK1_FT_150_SS00___bm_bm_bit_67'+'KL_VBHH_FT_200_SS00___bm_bm_bit_84'+'KL_KTLT_FT_202_SS00___bm_bm_bit_85'+'KL_RBAP_FT_203_SS00___bm_bm_bit_82'+'KL_RBAP_FT_203_SS00___bm_bm_bit_86'+'KL_RBAP_FT_203_SS00___bm_bm_bit_90'+'KL_RBAP_FT_203_SS00___bm_bm_bit_95'"));
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
            expected = @"\\ELAK\diagnostika and (\\ELAK\206.VW12.pozice or \\ELAK\206.VW12.cil)";
            if (!result.Equals(expected))
                Assert.Fail("Bad result {0} - {1}", result, expected);

            result = WinCCConvertor.Translate("('UserAdmin' || 'UserDisp') && ( ('207_EP09_F4_test_PIN' == 0x3838) || ('207_EP09_F4_test_typ' == 0x30303030) || ('207_EP09_F4_test_typ' == 0x20202020) || ('207_EP09_F4_test_barva' == 0x30303030) || ('207_EP09_F4_test_barva' == 0x20202020))");
            expected = @"(\\ELAK\UserAdmin or \\ELAK\UserDisp) and ((\\ELAK\207.EP09.F4.test.PIN eq 0x3838) or (\\ELAK\207.EP09.F4.test.typ eq 0x30303030) or (\\ELAK\207.EP09.F4.test.typ eq 0x20202020) or (\\ELAK\207.EP09.F4.test.barva eq 0x30303030) or (\\ELAK\207.EP09.F4.test.barva eq 0x20202020))";
            if (!result.Equals(expected))
                Assert.Fail("Bad result {0} - {1}", result, expected);
        }
    }
}