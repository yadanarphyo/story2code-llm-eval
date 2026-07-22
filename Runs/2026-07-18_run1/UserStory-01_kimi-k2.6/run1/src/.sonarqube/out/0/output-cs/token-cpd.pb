Õ
ö/Users/yadanarphyo/Space/ESS/Dissertation/Story2Code/Runs/2026-07-18_run1/UserStory-01_kimi-k2.6/run1/src/Services/IGetNearbyRecyclingFacilitiesService.cs
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
} π
ô/Users/yadanarphyo/Space/ESS/Dissertation/Story2Code/Runs/2026-07-18_run1/UserStory-01_kimi-k2.6/run1/src/Services/GetNearbyRecyclingFacilitiesService.cs
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
= 
[ 	
new 
RecyclingFacility !
{ 

FacilityId 
= 
$num 
, 
Name 
= 
$str 2
,2 3
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
$str (
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
$str 4
,4 5
Address 
= 
$str '
,' (
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
$str!! (
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
$str&& *
,&&* +
Address'' 
='' 
$str'' '
,''' (
City(( 
=(( 
$str(( $
,(($ %
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
$num++" %
,++% &
PhoneNumber,, 
=,, 
$str,, (
}-- 
].. 	
;..	 

}// 
public11 

IEnumerable11 
<11 
RecyclingFacility11 (
>11( )(
GetNearbyRecyclingFacilities11* F
(11F G
string11G M
zipCode11N U
)11U V
{22 
if33 

(33 
string33 
.33 
IsNullOrWhiteSpace33 %
(33% &
zipCode33& -
)33- .
)33. /
{44 	
return55 

Enumerable55 
.55 
Empty55 #
<55# $
RecyclingFacility55$ 5
>555 6
(556 7
)557 8
;558 9
}66 	
return88 
_facilities88 
.88 
Where88  
(88  !
f88! "
=>88# %
f88& '
.88' (
ZipCode88( /
==880 2
zipCode883 :
)88: ;
.88; <
ToList88< B
(88B C
)88C D
;88D E
}99 
}:: á

t/Users/yadanarphyo/Space/ESS/Dissertation/Story2Code/Runs/2026-07-18_run1/UserStory-01_kimi-k2.6/run1/src/Program.cs
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
 
UseHttpsRedirection

 
(

 
)

 
;

 
app 
. 
UseAuthorization 
( 
) 
; 
app 
. 
MapControllers 
( 
) 
; 
app 
. 
Run 
( 
) 	
;	 
ù
Ö/Users/yadanarphyo/Space/ESS/Dissertation/Story2Code/Runs/2026-07-18_run1/UserStory-01_kimi-k2.6/run1/src/Models/RecyclingFacility.cs
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
} „
ñ/Users/yadanarphyo/Space/ESS/Dissertation/Story2Code/Runs/2026-07-18_run1/UserStory-01_kimi-k2.6/run1/src/Controllers/RecyclingFacilitiesController.cs
	namespace 	
Implementation
 
. 
Controllers $
;$ %
[ 
ApiController 
] 
[ 
Route 
( 
$str !
)! "
]" #
public		 
class		 )
RecyclingFacilitiesController		 *
:		+ ,
ControllerBase		- ;
{

 
private 
readonly 0
$IGetNearbyRecyclingFacilitiesService 9
_service: B
;B C
public 
)
RecyclingFacilitiesController (
(( )0
$IGetNearbyRecyclingFacilitiesService) M
serviceN U
)U V
{ 
_service 
= 
service 
; 
} 
[ 
HttpGet 
] 
public 

ActionResult 
< 
IEnumerable #
<# $
RecyclingFacility$ 5
>5 6
>6 7
Get8 ;
(; <
[< =
	FromQuery= F
]F G
stringH N
zipCodeO V
)V W
{ 
var 
result 
= 
_service 
. (
GetNearbyRecyclingFacilities :
(: ;
zipCode; B
)B C
;C D
return 
Ok 
( 
result 
) 
; 
} 
} 