using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SpreadSheetData
{
    public static SpreadSheetData instance;

    private SheetsService service = null;
    private const string sheet_id = "1bCc0ErDdxERtunDC-Sv_OzGZxkFW5gesWNvYdAs3JMs";
    private const string sheet_name = "Calculator";
    private const string google_id = "808142315075-cnaofltl4hlpjdjoup2rc775j7ojsfev.apps.googleusercontent.com";
    private const string google_secret = "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";
    private bool is_credential = false;

    UserCredential credential;

    public SpreadSheetData()
    {
        instance = this;
        if (UserManager.instance.GetContribute())
            Login();
        /*
        IList<IList<System.Object>> form;
        SelectData("C3:D74", out form);
        for (int i = 0; i < form.Count; ++i)
        {
            for (int j = 0; j < form[i].Count; ++j)
                Debug.Log(form[i][j]);
        }
        InsertData("C3:D74", form);*/
    }


    public void Login()
    {
        if (!is_credential)
        {
            ClientSecrets secrets = new ClientSecrets()
            {
                ClientId = google_id,
                ClientSecret = google_secret
            };
            string[] scope = { SheetsService.Scope.Spreadsheets };

            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(secrets, scope, "user", System.Threading.CancellationToken.None).Result;
            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Kimchi"
            });
            is_credential = true;
        }
    }

    public void SelectData(string str_column_and_row, out IList<IList<object>> out_data)
    {
        if (!is_credential)
            Login();

        var request = service.Spreadsheets.Values.Get(sheet_id, sheet_name + "!" + str_column_and_row);

        ValueRange response = request.Execute();
        out_data = response.Values;
    }

    public void InsertData(string str_column_and_row, IList<IList<object>> list_data)
    {
        if (!is_credential)
            Login();

        var valueRange = new ValueRange()
        {
            MajorDimension = "ROWS",                    // ROWS or COLUMNS
            Values = list_data // 추가할 데이터
        };

        var update = service.Spreadsheets.Values.Update(valueRange, sheet_id, sheet_name + "!" + str_column_and_row);
        update.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
        update.Execute();
    }
}
