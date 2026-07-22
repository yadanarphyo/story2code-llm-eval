Õ
ö/Users/yadanarphyo/Space/ESS/Dissertation/Story2Code/Runs/2026-07-18_run2/UserStory-01_kimi-k2.6/run2/src/Services/IGetNearbyRecyclingFacilitiesService.cs
	namespace 	
Implementation
 
. 
Services !
;! "
public 
	interface 0
$IGetNearbyRecyclingFacilitiesService 5
{ 
IEnumerable 
< 
RecyclingFacility !
>! "(
GetNearbyRecyclingFacilities# ?
(? @
string@ F
zipCodeG N
)N O
;O P
} é+
ô/Users/yadanarphyo/Space/ESS/Dissertation/Story2Code/Runs/2026-07-18_run2/UserStory-01_kimi-k2.6/run2/src/Services/GetNearbyRecyclingFacilitiesService.cs
	namespace 	
Implementation
 
. 
Services !
;! "
public 
class /
#GetNearbyRecyclingFacilitiesService 0
:1 20
$IGetNearbyRecyclingFacilitiesService3 W
{ 
private 
readonly 
List 
< 
RecyclingFacility +
>+ ,
_facilities- 8
;8 9
public		 
/
#GetNearbyRecyclingFacilitiesService		 .
(		. /
)		/ 0
{

 
_facilities 
= 
new 
List 
< 
RecyclingFacility 0
>0 1
{ 	
new 
RecyclingFacility !
{ 

FacilityId 
= 
$num 
, 
Name 
= 
$str 5
,5 6
Address 
= 
$str '
,' (
City 
= 
$str $
,$ %
State 
= 
$str 
, 
ZipCode 
= 
$str !
,! "
DistanceInMiles 
=  !
$num" %
,% &
PhoneNumber 
= 
$str ,
} 
, 
new 
RecyclingFacility !
{ 

FacilityId 
= 
$num 
, 
Name 
= 
$str /
,/ 0
Address 
= 
$str &
,& '
City 
= 
$str $
,$ %
State 
= 
$str 
, 
ZipCode 
= 
$str !
,! "
DistanceInMiles   
=    !
$num  " %
,  % &
PhoneNumber!! 
=!! 
$str!! ,
}"" 
,"" 
new## 
RecyclingFacility## !
{$$ 

FacilityId%% 
=%% 
$num%% 
,%% 
Name&& 
=&& 
$str&& ,
,&&, -
Address'' 
='' 
$str'' '
,''' (
City(( 
=(( 
$str(( "
,((" #
State)) 
=)) 
$str)) 
,)) 
ZipCode** 
=** 
$str** !
,**! "
DistanceInMiles++ 
=++  !
$num++" &
,++& '
PhoneNumber,, 
=,, 
$str,, ,
}-- 
,-- 
new.. 
RecyclingFacility.. !
{// 

FacilityId00 
=00 
$num00 
,00 
Name11 
=11 
$str11 )
,11) *
Address22 
=22 
$str22 '
,22' (
City33 
=33 
$str33  
,33  !
State44 
=44 
$str44 
,44 
ZipCode55 
=55 
$str55 !
,55! "
DistanceInMiles66 
=66  !
$num66" %
,66% &
PhoneNumber77 
=77 
$str77 ,
}88 
,88 
new99 
RecyclingFacility99 !
{:: 

FacilityId;; 
=;; 
$num;; 
,;; 
Name<< 
=<< 
$str<< *
,<<* +
Address== 
=== 
$str== )
,==) *
City>> 
=>> 
$str>> $
,>>$ %
State?? 
=?? 
$str?? 
,?? 
ZipCode@@ 
=@@ 
$str@@ !
,@@! "
DistanceInMilesAA 
=AA  !
$numAA" %
,AA% &
PhoneNumberBB 
=BB 
$strBB ,
}CC 
}DD 	
;DD	 

}EE 
publicGG 

IEnumerableGG 
<GG 
RecyclingFacilityGG (
>GG( )(
GetNearbyRecyclingFacilitiesGG* F
(GGF G
stringGGG M
zipCodeGGN U
)GGU V
{HH 
ifII 

(II 
stringII 
.II 
IsNullOrWhiteSpaceII %
(II% &
zipCodeII& -
)II- .
)II. /
{JJ 	
returnKK 

EnumerableKK 
.KK 
EmptyKK #
<KK# $
RecyclingFacilityKK$ 5
>KK5 6
(KK6 7
)KK7 8
;KK8 9
}LL 	
varNN 
matchesNN 
=NN 
_facilitiesNN !
.OO 
WhereOO 
(OO 
fOO 
=>OO 
fOO 
.OO 
ZipCodeOO !
==OO" $
zipCodeOO% ,
)OO, -
.PP 
OrderByPP 
(PP 
fPP 
=>PP 
fPP 
.PP 
DistanceInMilesPP +
)PP+ ,
.QQ 
ToListQQ 
(QQ 
)QQ 
;QQ 
returnSS 
matchesSS 
.SS 
AnySS 
(SS 
)SS 
?SS 
matchesSS &
:SS' (

EnumerableSS) 3
.SS3 4
EmptySS4 9
<SS9 :
RecyclingFacilitySS: K
>SSK L
(SSL M
)SSM N
;SSN O
}TT 
}UU ≤
t/Users/yadanarphyo/Space/ESS/Dissertation/Story2Code/Runs/2026-07-18_run2/UserStory-01_kimi-k2.6/run2/src/Program.cs
var 
builder 
= 
WebApplication 
. 
CreateBuilder *
(* +
args+ /
)/ 0
;0 1
builder 
. 
Services 
. 
AddControllers 
(  
)  !
;! "
builder 
. 
Services 
. 
AddSingleton 
< 0
$IGetNearbyRecyclingFacilitiesService B
,B C/
#GetNearbyRecyclingFacilitiesServiceD g
>g h
(h i
)i j
;j k
var 
app 
= 	
builder
 
. 
Build 
( 
) 
; 
app

 
.

 
MapControllers

 
(

 
)

 
;

 
app 
. 
Run 
( 
) 	
;	 
ù
Ö/Users/yadanarphyo/Space/ESS/Dissertation/Story2Code/Runs/2026-07-18_run2/UserStory-01_kimi-k2.6/run2/src/Models/RecyclingFacility.cs
	namespace 	
Implementation
 
. 
Models 
;  
public 
class 
RecyclingFacility 
{ 
public 

int 

FacilityId 
{ 
get 
;  
set! $
;$ %
}& '
public 

string 
Name 
{ 
get 
; 
set !
;! "
}# $
=% &
string' -
.- .
Empty. 3
;3 4
public 

string 
Address 
{ 
get 
;  
set! $
;$ %
}& '
=( )
string* 0
.0 1
Empty1 6
;6 7
public 

string 
City 
{ 
get 
; 
set !
;! "
}# $
=% &
string' -
.- .
Empty. 3
;3 4
public		 

string		 
State		 
{		 
get		 
;		 
set		 "
;		" #
}		$ %
=		& '
string		( .
.		. /
Empty		/ 4
;		4 5
public

 

string

 
ZipCode

 
{

 
get

 
;

  
set

! $
;

$ %
}

& '
=

( )
string

* 0
.

0 1
Empty

1 6
;

6 7
public 

double 
DistanceInMiles !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 

string 
PhoneNumber 
{ 
get  #
;# $
set% (
;( )
}* +
=, -
string. 4
.4 5
Empty5 :
;: ;
} Ú
ñ/Users/yadanarphyo/Space/ESS/Dissertation/Story2Code/Runs/2026-07-18_run2/UserStory-01_kimi-k2.6/run2/src/Controllers/RecyclingFacilitiesController.cs
	namespace 	
Implementation
 
. 
Controllers $
;$ %
[ 
ApiController 
] 
[ 
Route 
( 
$str !
)! "
]" #
public 
class )
RecyclingFacilitiesController *
:+ ,
ControllerBase- ;
{		 
private

 
readonly

 0
$IGetNearbyRecyclingFacilitiesService

 9
_service

: B
;

B C
public 
)
RecyclingFacilitiesController (
(( )0
$IGetNearbyRecyclingFacilitiesService) M
serviceN U
)U V
{ 
_service 
= 
service 
; 
} 
[ 
HttpGet 
] 
public 

IActionResult 
Get 
( 
[ 
	FromQuery '
]' (
string) /
zipCode0 7
)7 8
{ 
var 
results 
= 
_service 
. (
GetNearbyRecyclingFacilities ;
(; <
zipCode< C
)C D
;D E
return 
Ok 
( 
results 
) 
; 
} 
} 