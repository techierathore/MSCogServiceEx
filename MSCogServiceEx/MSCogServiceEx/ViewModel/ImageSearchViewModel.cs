﻿using MSCogServiceEx.Model;
using MSCogServiceEx.Services;
using MSCogServiceEx.Services.BingSearch;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MSCogServiceEx.ViewModel
{
    public class ImageSearchViewModel : INotifyPropertyChanged
    {

        string searchString = "Cute Monkey";
        public string SearchString
        {
            get { return searchString; }
            set
            {
                searchString = value;
                OnPropertyChanged();
            }
        }
        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { isBusy = value; OnPropertyChanged(); }
        }

        List<ImageResult> searchResult;
        public List<ImageResult> Images
        {
            get { return searchResult; }
            set { searchResult = value; OnPropertyChanged(); }
        }
        ICommand getImages;
        public ICommand GetImagesCommand =>
                getImages ??
                (getImages = new Command(async () => await SearchForImages()));

        public async Task SearchForImages()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            //Bing Image API
            var url = $"https://api.cognitive.microsoft.com/bing/v5.0/images/" +
                      $"search?q={searchString}" +
                      $"&count=20&offset=0&mkt=en-us&safeSearch=Strict";

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", CognitiveServicesKeys.BingSearch);

                    var json = await client.GetStringAsync(url);

                    var result = JsonConvert.DeserializeObject<SearchResult>(json);

                    Images = result.Images.Select(i => new ImageResult
                    {
                        ContextLink = i.HostPageUrl,
                        FileFormat = i.EncodingFormat,
                        ImageLink = i.ContentUrl,
                        ThumbnailLink = i.ThumbnailUrl,
                        Title = i.Name
                    }).ToList();
                  
                }
            }
            catch (Exception ex)
            {
                //  ("Unable to query images: " + ex.Message);               
            }
            finally
            {
                IsBusy = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    
}
