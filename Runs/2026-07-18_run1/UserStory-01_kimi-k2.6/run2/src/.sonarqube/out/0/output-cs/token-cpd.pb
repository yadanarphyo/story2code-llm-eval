ƒ!
ô/Users/yadanarphyo/Space/ESS/Dissertation/Story2Code/Runs/2026-07-18_run1/UserStory-01_kimi-k2.6/run2/src/Services/GetNearbyRecyclingFacilitiesService.cs
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
$str&& 3
,&&3 4
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
$str11 .
,11. /
Address22 
=22 
$str22 &
,22& '
City33 
=33 
$str33 $
,33$ %
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
$str77 (
}88 
}99 	
;99	 

}:: 
public<< 

IEnumerable<< 
<<< 
RecyclingFacility<< (
><<( )(
GetNearbyRecyclingFacilities<<* F
(<<F G
string<<G M
zipCode<<N U
)<<U V
{== 
return>> 
_facilities>> 
.?? 
Where?? 
(?? 
f?? 
=>?? 
f?? 
.?? 
ZipCode?? !
.??! "
Equals??" (
(??( )
zipCode??) 0
,??0 1
StringComparison??2 B
.??B C
OrdinalIgnoreCase??C T
)??T U
)??U V
.@@ 
OrderBy@@ 
(@@ 
f@@ 
=>@@ 
f@@ 
.@@ 
DistanceInMiles@@ +
)@@+ ,
.AA 
ToListAA 
(AA 
)AA 
;AA 
}BB 
}CC Õ
ö/Users/yadanarphyo/Space/ESS/Dissertation/Story2Code/Runs/2026-07-18_run1/UserStory-01_kimi-k2.6/run2/src/Services/IGetNearbyRecyclingFacilitiesService.cs
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
} á

t/Users/yadanarphyo/Space/ESS/Dissertation/Story2Code/Runs/2026-07-18_run1/UserStory-01_kimi-k2.6/run2/src/Program.cs
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
Ö/Users/yadanarphyo/Space/ESS/Dissertation/Story2Code/Runs/2026-07-18_run1/UserStory-01_kimi-k2.6/run2/src/Models/RecyclingFacility.cs
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
} Â
ñ/Users/yadanarphyo/Space/ESS/Dissertation/Story2Code/Runs/2026-07-18_run1/UserStory-01_kimi-k2.6/run2/src/Controllers/RecyclingFacilitiesController.cs
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
var 
results 
= 
_service 
. (
GetNearbyRecyclingFacilities ;
(; <
zipCode< C
)C D
;D E
return 
Ok 
( 
results 
) 
; 
} 
} 