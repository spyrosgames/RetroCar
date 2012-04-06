var startTime: float;

var startPos: Vector2;

var couldBeSwipe: boolean;

var comfortZone: float;

var minSwipeDist: float;

var maxSwipeTime: float;

var car : Rigidbody;
private var globals : Globals;

function start() {
	globals = Globals.GetInstance();
}
 

function Update() {

    if (iPhoneInput.touchCount > 0) {

        var touch = iPhoneInput.touches[0];

        

        switch (touch.phase) {

            case iPhoneTouchPhase.Began:

                couldBeSwipe = true;

                startPos = touch.position;

                startTime = Time.time;

                break;

            

            case iPhoneTouchPhase.Moved:

                if (Mathf.Abs(touch.position.x - startPos.x) > comfortZone) {

                    couldBeSwipe = false;

                }

                break;

            

            case iPhoneTouchPhase.Stationary:

                couldBeSwipe = false;

                break;

            

            case iPhoneTouchPhase.Ended:

                var swipeTime = Time.time - startTime;

                var swipeDist = (touch.position - startPos).magnitude;

                

                if (couldBeSwipe && (swipeTime < maxSwipeTime) && (swipeDist > minSwipeDist)) {

                    // It's a swiiiiiiiiiiiipe!

                    var swipeDirection = Mathf.Sign(touch.position.x - startPos.x);

                    if(car != null)
					{
							//Moving to Right
							car.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
							car.position = Vector3(-12, car.position.y, -7.433446);
							globals.lastCarPositionY = car.position.y;
					}


                    // Do something here in reaction to the swipe.

                }

                break;

        }

    }
}

