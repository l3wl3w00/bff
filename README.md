## BFF SSO Demo
Ez a demo a BFF és az SSO együttes működését mutatja be.

Ez a demo 2 különböző alkalmazást szimulál, és összesen 6 különböző alkalmazás-komponensből áll, 
amik párhuzamosan futnak, és kommunikálnak egymással:
- Kliens 1 és Kliens 2
  - A `BffDemo.Bff1/BffDemo.Client1` és a `BffDemo.Bff2/BffDemo.Client2` mappa alatt találhatók
  - Ez 2 angular kliens, az előbbi a `localhost:4200` az utóbbi pedig a `localhost:4201` címen. 
  - Habár jelen esetben kódjukban szinte azonsak, ezek 2 teljesen független alkalmazásnak a klienseit szimulálják. 
- BFF 1 és BFF 2:
  - Ezek a BFF komponensei a 2 alkalmazásnak.
  - `localhost:5001` és `localhost:5002` címeken futnak.
  - Ezen template alapján készültek: https://github.com/DuendeSoftware/IdentityServer.Templates/tree/main/src/BffRemoteApi
  - Kódjukban ezek is nagyon hasonlóak, azonban ezek is 2 teljesen független alkalmazásnak a BFF komponenseit szimulálják
- Identity Server
  - Ez maga az identity server, ami bejelentkezteti a felhasználókat, és szolgáltatja a tokeneket.
    - Ezen template alapján készült: https://github.com/DuendeSoftware/IdentityServer.Templates/tree/main/src/IdentityServerInMem
  - `localhost:5000` címen fut 
- Backend
  - Ez pedig egy egyszerű ASP.NET backend WEB API, amit mindkét alkalmazás authentikáltan hív
  - `localhost:6000` címen fut

Az alábbi ábra szemlélteti azt a folyamatot, ami által a felhasználó mindkét alkalmazásba bejelentkezik, de az adatait csak az első alkalommal adja meg:
![folyamat.png](docs/folyamat.png)

Az ábrában sorszámozva vannak a kérések, ezek olyan sorrendben hajtódnak végre. Az "1."-tal kezdődő sorszámmal rendelkező 
kérések jelentik az első alkalmazás bejelentkezési folyamatának a részét, a "2."-tal kezdődőek pedig a másodikét.