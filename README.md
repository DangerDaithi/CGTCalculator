A .NET core console application that calculates Capital Gains Tax for Restricted Stock Unit sells.

To use in command line e.g.:

```
.\CGTRSUCalculator.exe --buy-price 79.45 --sell-price 87.45 --sell-date 25/10/2019 --number-of-assets-sold 25 --convert-from-currency USD --convert-to-currency EUR
```

```
.\CGTRSUCalculator.exe --buy-price 79.45 --sell-price 87.45 --sell-date 25/10/2019 --number-of-assets-sold 25 --convert-from-currency USD --convert-to-currency EUR --remaining-personal-threshold 500
```

For command line options use --help:
```
.\CGTRSUCalculator.exe --help
```

Supported currencies are EUR, USD and GBP

