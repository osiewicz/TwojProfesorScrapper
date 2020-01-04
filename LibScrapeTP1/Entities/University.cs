using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;

namespace LibScrapeTP.Entities
{
    public enum University
    {
        pb, // Politechnika Białystocka
        uwb, // Uniwersytet w Białymstoku

        utp, // Uniwersytet Technologiczno-Przyrodniczy
        ukw, // Uniwersytet Kazimierza Wielkiego

        ajd, // Akademia im. Jana Długosza
        pcz, // Politechnika Częstochowska

        ug, // Uniwersytet Gdański
        pg, // Politechnika Gdańska

        us, // Uniwersytet Śląski
        ueka, // Uniwersytet Ekonomiczny w Katowicach
        polsl, // Politechnika Śląska

        agh, // Akademia Górniczo-Hutnicza im. Stanisława Staszica
        uj, // Uniwersytet Jagielloński
        pk, // Politechnika Krakowska
        uek, // Uniwersytet Ekonomiczny
        ur, // Uniwersytet Rolniczy
        upk, // Uniwersytet Pedagogiczny
        ka, // Krakowska Akademia im. Andrzeja Frycza Modrzewskiego

        umcs, // Uniwersytet Marii Curie-Skłodowskiej
        upl, // Uniwersytet Przyrodniczy w Lublinie
        am, // Uniwersytet Medyczny w Lublinie

        uni, // Uniwersytet Łódzki
        umed, // Uniwersytet Medyczny w Łodzi

        uwm, // Uniwersytet Warmińsko-Mazurski

        uo, // Uniwersytet Opolski
        po, // Politechnika Opolska

        uam, // Uniwersytet im. Adama Mickiewicza w Poznaniu
        put, // Politechnika Poznańska
        upp, // Uniwersytet Przyrodniczy w Poznaniu
        uep, // Uniwersytet Ekonomiczny w Poznaniu
        ump, // Uniwersytet Medyczny w Poznaniu

        urz, // Uniwersytet Rzeszowski
        prz, // Politechnika Rzeszowska

        uw, // Uniwersytet Warszawski
        wat, // Wojskowa Akademia Techniczna
        pw, // Politechnika Warszawska
        aps, // Akademia Pedagogiki Specjalnej im. Marii Grzegorzewskiej
        sgh, // Szkoła Główna Handlowa
        wum, // Warszawski Uniwersytet Medyczny

        pwr, // Politechnika Wrocławska
        uwr, // Uniwersytet Wrocławski
        up,  // Uniwersytet Przyrodniczy
        uew, // Uniwersytet Ekonomiczny 

        uph, // Uniwersytet Przyrodniczo-Humanistyczny (Siedlce)
        umk, // Uniwersytet Mikołaja Kopernika
        kpsz, // Politechnika Koszalińska
        PSk, // Politechnika Świętokrzyska
        pwsz, // Państwowa Wyższa Szkoła Zawodowa (Kalisz)
        wsze, // Państwowa Wyższa Szkoła Zawodowa w Elblągu
        pwsip, // Państowa Wyższa Szkoła Informatyki i Przedsiębiorczości (Łomża)
    }

    public static partial class Helpers
    {
        public static string UniversityToUrlString(University uni)
        {
            
            return uni.ToString();
        }

        public class EnumToIntConvHelper<TEnumeration> where TEnumeration : Enum
        {
            public static int Convert(TEnumeration enumeration)
            {
                return System.Convert.ToInt32(enumeration);
            }
        }
    }
}
