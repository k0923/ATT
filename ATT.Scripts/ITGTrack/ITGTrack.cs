﻿using ATT.Data;
using ATT.Data.Entity;
using SAPAutomation;
using SAPFEWSELib;
using ScriptRunner.Interface;
using ScriptRunner.Interface.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;

namespace ATT.Scripts
{
    [Script("Get EDI Keys")]
    public class ITGTrack : ScriptBase<ITGTrackData>
    {

        private ATT.Data.AttDbContext db;
  

        [Step(Id = 1, Name = "Login to SAP LH ITG")]
        public void Login() {
            UIHelper.Login(_data);
        }

        [Step(Id = 2, Name = "Get Message Ids")]
        public void GetMessageId() {

            _data.NewGuid();

            SAPTestHelper.Current.SAPGuiSession.StartTransaction("ZIDOCAUDREP");
            if (SAPTestHelper.Current.PopupWindow != null) {
                SAPTestHelper.Current.PopupWindow.FindDescendantByProperty<GuiRadioButton>((r => r.Text.Contains("Continue With"))).Select();
                SAPTestHelper.Current.PopupWindow.FindDescendantByProperty<GuiButton>(r => r.Text.Contains("Confirm Selection")).Press();
            }


            // Fill Control/Status Record Search
            SAPTestHelper.Current.MainWindow.FindByName<GuiCTextField>("S_CREDAT-LOW").Text = _data.GetStart().ToString("MM/dd/yyyy");
            
            SAPTestHelper.Current.MainWindow.FindByName<GuiCTextField>("S_CREDAT-HIGH").Text = _data.GetEnd().ToString("MM/dd/yyyy");


            SAPTestHelper.Current.MainWindow.FindByName<GuiCTextField>("S_CRETIM-LOW").Text = _data.GetStart().ToString("HH:mm:ss");
            SAPTestHelper.Current.MainWindow.FindByName<GuiCTextField>("S_CRETIM-HIGH").Text = _data.GetEnd().ToString("HH:mm:ss");


            SAPTestHelper.Current.MainWindow.FindByName<GuiCTextField>("S_STATUS-LOW").Text = _data.IDocStatus;
            SAPTestHelper.Current.MainWindow.FindByName<GuiCTextField>("P_DIRECT").Text = "2";

            // Fill Message Type
            SAPTestHelper.Current.MainWindow.FindByName<GuiRadioButton>("R_ACCDOC").Select();
            SAPTestHelper.Current.MainWindow.FindByName<GuiCTextField>("S_MESTYP-LOW").Text = "ACC_DOCUMENT";

            //Fill IDoc Data Segments
            SAPTestHelper.Current.MainWindow.FindByName<GuiTextField>("S_USNAM-LOW").Text = _data.UserId;

            //Fill Download Option
            SAPTestHelper.Current.MainWindow.FindByName<GuiCheckBox>("P_DOWNLD").Selected = true;

            SAPTestHelper.Current.MainWindow.FindByName<GuiTextField>("P_CPATH").Text = _data.File;

            //Report Output Options
            SAPTestHelper.Current.MainWindow.FindByName<GuiRadioButton>("P_DOC").Select();

            SAPTestHelper.Current.MainWindow.FindByName<GuiButton>("btn[8]").Press();

            if (SAPTestHelper.Current.PopupWindow != null)
                SAPTestHelper.Current.PopupWindow.FindByName<GuiButton>("btn[0]").Press();


            SAPTestHelper.Current.CloseSession();
        }


        [Step(Id = 3,Name ="Upload File")]
        public void UploadFile() {
            if (File.Exists(_data.File)) {
                var msgId_ITGs = Tools.GetDataEntites<MsgIDs_ITG>(_data.File);
                using(db = new AttDbContext()) {
                    db.MsgIDs_ITG.AddRange(msgId_ITGs);
                    db.SaveChanges();
                }
                File.Delete(_data.File);
            }
        }

        [Step(Id = 4, Name = "Update Schedule Info")]
        public void UpdateSchedule() {
            _data.Start = _data.Start.GetNext(_data.Interval);
        }
    }
}
