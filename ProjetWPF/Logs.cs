﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasySave.ControllerLib;
using EasySave.IviewLib;
using EasySave.ModelLib;
using System.Windows.Controls;
using System.Windows;

namespace ProjetWPF
{
    public class Logs
    {
        public async Task DisplayLogs()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            var logs = (TextBox)mainWindow.FindName("History_t");
            try
            {

                string content = File.ReadAllText(GlobalVariables.LogFilePath);
                logs.Text = content;
            }
            catch (Exception ex)
            {
                logs.Text = await Translation.Instance.Translate("Une erreur est survenue lors de la lecture du fichier : " + ex.Message);

            }
        }
    }
}
