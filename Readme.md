![App Screenshot](AppScreenshot.png)

This app will let you create a Spotify playlist based on tracks listed in a web page. You will need to come up with an XPath expression that will extract the track names from the web page. Useful links:


[W3Wchools XPath](https://www.w3schools.com/xml/xpath_intro.asp)

[VSCode XPath Tester Extension](https://marketplace.visualstudio.com/items?itemName=creinbacher.xpathtester)


# To use the app

## Step 1 - Create a Spotify app

* Go to https://developer.spotify.com/dashboard/applications
* Create an application and make a note of the Client ID
* Add a redirect URI of http://127.0.0.1/ (This is so when you authenticate via your browser it can post the access token back to the app)

## Step 2 - Configure

The configuration file is appsettings.json

|Setting|Description|
|-------|-----------|
|SpotifyUsername|Spotify username, regular or email|
|SpotifyClientId|The Client ID of the app you created in your Spotify Developer Console|
|WebPlaylists|A collection of web playlists containing details of source playlists|

### WebPlaylist

```json
{
  "Url": "https://www.bbc.co.uk/programmes/articles/5JDPyPdDGs3yCLdtPhGgWM7/bbc-radio-6-music-playlist",
  "TrackNamesXPath": "(//div[@class='text--prose']/p)[position()>1]"
}
```
|Property|Description|
|-------|-----------|
|Url|URL of a HTML or XML based playlist|
|TrackNamesXPath|XPath expression to select nodes containing the track names. Inner text will be extracted from each node.|

## Step 3 - Run

Run WebPlaylistToSpotify.exe

