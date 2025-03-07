# BFF SSO Demo
Ez a demo a BFF és az SSO együttes működését mutatja be.

## Komponensek
Ez a demo 3 különböző alkalmazást szimulál, és összesen 8 különböző alkalmazás-komponensből áll, 
amik párhuzamosan futnak, és kommunikálnak egymással:
- Kliensek
  - A `BffDemo.Bff1/BffDemo.Client1` és a `BffDemo.Bff2/BffDemo.Client2` mappa alatt találhatók a BFF kliensek
  - A `BffDemo.NoBffApplication/BffDemo.NoBffClient` mappa alatt található a BFF nélüli kliens
  - Ezek angular kliensek
  - A `bff-client-1.test:4201` és a `bff-client-2.test:4202` címen vannak a BFF kliensek és `no-bff-client.test:4203` címen található a BFF nélküli app kliense. 
  - Habár jelen esetben kódjukban szinte azonsak, ezek teljesen független alkalmazásnak a klienseit szimulálják. 
- BFF 1 és BFF 2:
  - Ezek a BFF komponensei a 2 BFF alkalmazásnak.
  - `bff-server-1.test:5001` és `bff-server-2.test:5002` címeken futnak.
  - Ezen template alapján készültek: https://github.com/DuendeSoftware/IdentityServer.Templates/tree/main/src/BffRemoteApi
  - Kódjukban ezek is nagyon hasonlóak, azonban ezek is 2 teljesen független alkalmazásnak a BFF komponenseit szimulálják
  - A kéréseket továbbítják a backend felé, pontosabban minden `localhost:5001/bff1/api1/[request]` vagy `localhost:5002/bff2/api2/[request]` címre érkező kérést továbbítanak a `localhost:6000/[request]` címre. Ez konfigurálható az `appsettings.json` file-ban. 
- Identity Server
  - Ez maga az identity server, ami bejelentkezteti a felhasználókat, és szolgáltatja a tokeneket.
  - Ezen template alapján készült: https://github.com/DuendeSoftware/IdentityServer.Templates/tree/main/src/IdentityServerInMem
  - `localhost:5000` címen fut
- Backendek
  - Ezek pedig egy egyszerű ASP.NET backend WEB API-k, amiket az alkalmazások authentikáltan hívnak
  - `localhost:6000` és `localhost:6001` címeken futnak
## Folyamat

Az alábbi ábra szemlélteti azt a folyamatot, ami által a felhasználó mindkét alkalmazásba bejelentkezik, de az adatait csak az első alkalommal adja meg:
![folyamat.png](docs/folyamat.png)

Az ábrában sorszámozva vannak a kérések, ezek olyan sorrendben hajtódnak végre. Az "1."-tal kezdődő sorszámmal rendelkező 
kérések jelentik az első alkalmazás bejelentkezési folyamatának a részét, a "2."-tal kezdődőek pedig a másodikét.

## Felhasználók

A teszt felhasználók a Duende által biztosított templateből származnak. 2 ilyen Felhasználó van:
- Alice
  - user: alice
  - pass: alice
- Bob
  - user: bob
  - pass: bob

## Futtatás előtt
Lokális futtatás előtt érdemes lehet az alábbi 4 sort beilleszteni a hosts file-ba, mert enélkül nekem nem működik:
- Windowson: `C:\Windows\System32\drivers\etc\hosts`
- Linux/MaxOS-en: `/etc/hosts`
```
127.0.0.1   bff-server-1.test
127.0.0.1   bff-server-2.test
127.0.0.1   bff-client-1.test
127.0.0.1   bff-client-2.test

127.0.0.1   no-bff-client.test
```
Ezzel lényegében minden kliens és server külön domainen fut, és ezért élethűbben szimulálja a valóságot

## Indítás
A legelső indítás előtt:
- Navigáljunk el a `BffDemo.NoBffApplication/BffDemo.NoBffClient` mappába, és futtassuk az `npm install` parancsot

A teljes folyamat szimulálásához mind a 8 komponenst külön-külön el kell indítani.
- A BFF-eket, az Identity Servert és a Backend-et szokásos .NET projektként kell futtatni
  - Visual Studioban jobb klikk a projektre -> Debug -> Start New Instance
- A két angular klienst az alábbi módon érdemes futtatni:
  - Navigáljunk el a `BffDemo.Bff1/BffDemo.Client1`, a `BffDemo.Bff2/BffDemo.Client2` vagy a `BffDemo.NoBffApplication/BffDemo.NoBffClient` mappába,
  - majd futtasuk az `npm start` parancsot

Készült egy script, ami automatizálja az indítást, ez a gyökér mappában a `start.ps1` című file. Ezt a scriptet futtatva elindul minden szükséges alkalmazás.
Az alkalmazások elindítása után a script user inputot vár a terminálba, ami 2 féle lehet:
- 'q' betű beütése után minden futó alkalmazást bezár
- 'r' betű leütése után a .NET alkalmazásokat újraindítja (az angularokat nem kell, mert azok maguktól frissülnek)

A fenti script azonban akkor nem hasznos, ha debuggolni akarjuk a .NET projekteket. Ebben az esetben érdemes külön-külön debug módban elindatani a szerver oldali kódot, majd a `start-angular.ps1` scriptet futtatni, ami a frontendeket indítja el.
