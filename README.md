# RATP API made simple using REST Webservices.

## How to get the realtime schedules?

1. **Get the lines**
    1. Sample request for metros : http://restratpws.azurewebsites.net/api/lines/metro
    1. Sample response 
      ```
        [
          {
            "id": "100110001",
            "name": "La Défense / Château de Vincennes",
            "shortName": "M1",
            "image": "m1.gif"
          },
          {
            "id": "100110002",
            "name": "Porte Dauphine / Nation",
            "shortName": "M2",
            "image": "m2.gif"
          },
          {
            "id": "100110003",
            "name": "Pont de Levallois Bécon / Gallieni",
            "shortName": "M3",
            "image": "m3.gif"
          },
          ...
        ]
      ```
1. **Get the directions**
    1. Sample request for M1 : http://restratpws.azurewebsites.net/api/directions/100110001
    1. Sample response 
      ```
      [
        {
          "way": "A",
          "name": "La Défense"
        },
        {
          "way": "R",
          "name": "Château de Vincennes"
        }
      ]
      ```
1. **Get the stations**
    1. Sample request for M1 : http://restratpws.azurewebsites.net/api/directions/100110001
    1. Sample response 
      ```
      [
        {
          "id": "114",
          "name": "Argentine"
        },
        {
          "id": "133",
          "name": "Bastille"
        },
        {
          "id": "137",
          "name": "Bérault"
        },
        {
          "id": "190",
          "name": "Champs-Elysées - Clémenceau"
        },
        ...
      ]
      ```
1. **Get the realtime schedules**
    1. Sample request for M1 from Argentine towards La Défense : http://restratpws.azurewebsites.net/api/missions/100110001/from/114/way/a
    1. Sample response 
      ```
      [
        "1 mn",
        "3 mn",
        "7 mn",
        "10 mn"
      ]
      ```

## Extras

* Get the lines and networks images

    * For the network's image use :
      ```
      p_met.gif
      p_bus.gif
      p_rer.gif
      p_tram.gif
      ```
    * For the line's image use 'image' value received from api/lines/{networkId}
      
    * Samples :

      http://restratpws.azurewebsites.net/api/images/p_met.gif
      
      http://restratpws.azurewebsites.net/api/images/m1.gif
