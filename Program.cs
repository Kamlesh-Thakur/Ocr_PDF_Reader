using IronOcr;

IronTesseract ocr = new();

//Keys for locating data
var keys = new List<string>() { "From:", "To:" };

//Ignore the page if the page does not begin with
var ignorePageKey = "FMS";

//List to hold flight records
List<FlightDetail> flightDetails = new();

using (var ocrInput = new OcrInput())
{
    string fileLocation = GetFileLocation();

    ocrInput.AddImage(fileLocation);

    var ocrResult = ocr.Read(ocrInput);

    List<OcrResult.Page> relevantPagesOnly = FilterPages(ignorePageKey, ocrResult);

    ParseFileAndAssignValues(keys, flightDetails, relevantPagesOnly);

    foreach (var flightDetail in flightDetails)
        Console.WriteLine(flightDetail);
    Console.ReadKey();
}

static string GetFileLocation()
{
    var baseDirectory = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
    var rootpath = Directory.GetParent(Directory.GetParent(Directory.GetParent(baseDirectory).FullName).FullName).FullName;
    var fileLocation = Path.Combine(rootpath, "Values.png");
    return fileLocation;
}

static List<OcrResult.Page> FilterPages(string ignorePageKey, OcrResult ocrResult)
{
    return ocrResult.Pages.Where(o => o.Text.Trim().StartsWith(ignorePageKey)).ToList();
}

static void ParseFileAndAssignValues(List<string> keys, List<FlightDetail> flightDetails, List<OcrResult.Page> relevantPagesOnly)
{
    foreach (var page in relevantPagesOnly)
    {
        FlightDetail record = new();

        foreach (var key in keys)
        {
            var data = page.Lines.FirstOrDefault(o => o.Text.StartsWith(key)).Text;
            if (string.IsNullOrEmpty(data)) continue;
            var value = data
                .Replace(key, "")
                .Replace("|", " ")
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)[0];

            _ = key switch
            {
                "From:" => record.From = value,
                "To:" => record.To = value,
                _ => throw new ArgumentException("Invalid string value for key", nameof(key)),
            };
        }
        flightDetails.Add(record);
    }
}
