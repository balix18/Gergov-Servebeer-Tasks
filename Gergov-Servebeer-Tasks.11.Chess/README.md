# Gergov-Servebeer-Tasks.10.Chess

Egy sakktáblán egy királyt, két futót és két bástyát helyezünk el az placement.csv fájl tartalmának megfelelõen.
(Minden sorban a figura típusa és pozíciójának koordinátái találhatók meg.)

Tipp: a feladat megoldása elõtt keressetek egy képet egy sakktábláról a neten, és rajzoljátok rá a bemeneti fájlban leírt elhelyezést. Ez segít a megoldás kigondolásában és az ellenõrzésben is.

## 1. Feladat
Töltsd be az placement.csv fájl tartalmát, és tárold el a memóriában a további feladatok megoldására alkalmas formában!

(Tipp: 2D tömb, int vagy enum típusú mezõkkel. Minden cellában az adott mezõn álló bábura utaló értéket kell tárolni, pl. Király:0, Futó: 1, Bástya:2. A 0-1 alapú indexelés és a betû-index átváltást célszerû egy külön függvényben megvalósítani. Pl. A,1 -> 0, 0 ; C,2 -> 2, 1)

## 2. Feladat
Írj függvényt, amely megállapítja, hogy egy paraméterként megadott mezõn álló bástya üt-e a táblán másik figurát!

(Tipp: a függvény paraméterei legyenek a mezõ indexei. Az ellenõrzéshez egy ciklussal végig kell járni a megadott mezõtõl balra, jobbra, felette illetve alatta található cellákat. Ha bármelyik cellára igaz, hogy nem üres, akkor üt a bástya. A függvény visszatérési értéke legyen bool.)

## 3. Feladat
Írj függvényt, amely megállapítja, hogy egy paraméterként megadott mezõn álló futó üt-e a táblán másik figurát!

(Tipp: mint az elõbb, csak itt nem jobbra-balra-le-fel kell vizsgálódni, hanem az átlókon - tehát a ciklusban egyszerre mindkét koordinátát kell léptetni, az iránytól függõen le vagy fel.)

## 4. Feladat
Az elõzõekben megírt függvények felhasználásával írj egy olyan függvényt, amely megállapítja, hogy sakkban van-e a táblán elhelyezett király.

(Tipp: egymásba ágyazott ciklusokkal végig kell járni a tömböt, ha egy mezõn áll figura, meghívni a neki megfelelõ ütést vizsgáló függvényt. Ha bármelyik figura üti a királyt, akkor sakkban van.)

## 5. Feladat
Van-e olyan figura a táblán, amit a király le tud ütni?

(Tipp: Itt két dolgot kell vizsgálni. Egyrészt, hogy van-e olyan figura, amit a király elér, azaz áll-e bármelyik közvetlenül vagy átlósan szomszédos mezõn figura, és ha igen, akkor arra a mezõre lépve a király nem kerül-e sakkba.
A "körülnézéshez" a -1, 0, 1 koordináta kombinációkat kell végignézni a 0,0 kivételével. Az új helyen sakkba lépést a 4. feladatban megírt függvény segítségével lehet vizsgálni.)
