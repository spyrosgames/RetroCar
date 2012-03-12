/** @mainpage 
<div id="header">
	<div id="container">
		<div class="logo"> 
@image html of_devSupport.png 
		</div>
    </div>
</div>
			<h3>Platform: OpenFeint X SDK 2.0.3</h3>
			<h4>Readme.html for OpenFeint X SDK 2.0.3<br>Release date 07.15.2011 <br>
				Release Notes Copyright (c) 2009-2011 OpenFeint Inc. <br>
				All Rights Reserved. 
			</h4>

<div id="importable"> 

<h4>In This Document</h4>
<ul>
<li><a href="#more">For more information</a></li>
<li><a href="#new">What's New</a></li>
<li><a href="#integrate">To integrate OpenFeintX into your iOS game</a></li>
<li><a href="#sample_app">Build and run the OpenFeintX Sample Application</a></li>
<li><a href="#integrating">Integrate OpenFeint X with your application</a></li>
<li><a href="#using">Using Downloadable Content</a></li>
<li><a href="#changelog">Change Log</a></li>
</ul>

<h4><a name="more"></a>For more information</h4>
<ul>
<li>The home page for information about OpenFeint X is <a href="http://support.openfeint.com/dev/ofx-introduction/">here</a>.</li>
<li>A complete set of the OpenFeint X 2.0.3 API Documentation can be seen at 
<a href="http://support.openfeint.com/images/ref/2.0.3-OFX-iOS/APIdocumentation/index.html">http://support.openfeint.com/images/ref/2.0.3-OFX-iOS/APIdocumentation/index.html</a>.</li>
<li>A zipped package containing this README can be downloaded <a href="http://support.openfeint.com/images/ref/2.0.3-OFX-iOS/OF2.0.3_APIdocumentation.zip">here</a>.</li>
</ul>

<h4><a name="new"></a>What's New</h4>
<ul>
<li>OFX is now OFSDK-2.12 and GameFeed compatible.</li>
<li>OFX now supports versioning of all store items. For games using OFX versions prior to 2.0.3, the server will assume the game version is 1.0 and filter out any 
assets with a start version higher than that. You can use versioning to add and test new assets that are not visible to live versions of the live game.</li>
<li>You no longer need to use Objective C++ to compile OFX; Objective C is sufficient.</li>
</ul>
<h4><a name="integrate"></a>To integrate OpenFeintX into your iOS game:</h4> 
<ol> 
<li><a href="#sample_app">Build and run the OpenFeintX Sample Application</a></li> 
<li><a href="#integrating">Integrate OpenFeint X with your application</a></li> 
<li><a  href="#using">Using Downloadable Content</a></li> 
</ol> 
<hr/> 
For more information about getting started with OpenFeint X, please see <a href="http://support.openfeint.com/dev/ofx-introduction/">Introduction to OFX</a>.
<hr/> 
<p> 
<a href="mailto:documentation@openfeint.com?subject=Readme.html OFX 2.0.3"><i>Comments, Corrections?</i></a> 
</p> 
<h4><a name="sample_app"></a>Build and run the OpenFeintX Sample Application</h4> 
<ol> 
<li>Download and install the latest version of Apple's iOS SDK.
</li> 
<li>Download the latest version of OpenFeint iOS from <a href="http://api.openfeint.com/dd" target="_new">http://api.openfeint.com/dd</a>. </li> 
<li>Download the latest version of OpenFeintX iOS from <a href="http://api.openfeint.com/dd" target="_NEW">http://api.openfeint.com/dd</a>.
</li> 
<li>Extract the contents of the OpenFeint iOS .zip file.</li> 
<li>Extract the contents of the OpenFeintX .zip file. </li> 
<li>Copy the <b>OpenFeint</b> folder in<br /> 
  its entirety from the extracted OpenFeint iOS SDK to the <strong>openfeint/ </strong>directory in the extracted OpenFeintX folder. The directory<br /> 
  structure of the OpenFeintX folder should now look as follows: <strong>openfeint/OpenFeint/</strong>... (api, Resources, etc)
</li> 
<li>Log in to <a href="http://api.openfeint.com/dd" target="_NEW">http://api.openfeint.com/dd</a> and download your application's offline<br /> 
  settings file. Place this file in the OpenFeintX <strong>config</strong>/ directory.
</li> 
<li>Open <strong>sample/OpenFeintXSample.xcodeproj</strong>.
</li> 
<li>Open <strong>sample/Classes/OpenFeintXAppDelegate.mm.</strong></li> 
<li>Change the OpenFeint initialization to<br /> 
  include your Product Key, Secret, and Product Display Name from <a href="http://api.openfeint.com/dd" target="_NEW">http://api.openfeint.com/dd</a>.  It will look something like this:
<pre> 
  [OpenFeint
		initializeWithProductKey:@"<i>yourProductKey</i>"
		andSecret:@"<i>yourProductKey</i>"
		andDisplayName:@"<i>yourProductDisplayName</i>"
		andSettings:ofSettings
		andDelegates:ofDelegates];
  </pre> 
</li> 
<li>Build and Run!
</li> 
</ol> 
<h4><a name="integrating"></a>Integrate OpenFeint X with your application</h4> 
<ol> 
<li>If you have not already integrated the latest OpenFeint version into your application:
<ol type="a"> 
<li>Download the latest version of OpenFeint iOS from <a href="http://api.openfeint.com/dd" target="_new">http://api.openfeint.com/dd</a>. </li> 
<li>Follow the instructions in the package to integrate OpenFeint into your application.</li> 
</ol> 
</li> 
<li>Download the latest version of OpenFeintX iOS from <a href="http://api.openfeint.com/dd" target="_new">http://api.openfeint.com/dd</a>.
</li> 
<li>Extract the contents of the OpenFeintX .zip file.
  </li> 
<li>Open your application's project in XCode.
  </li> 
<li>Drag the <strong>source</strong> folder from the extracted OpenFeintX on to your <strong>Groups & Files</strong> window<br /> 
    in XCode.
    </li> 
<li>Select the <strong>Copy files to destination folder</strong> option.
  </li> 
<li>Make sure you are linking against the following frameworks:
<ul> 
<li>MobileCoreServices.framework</li> 
</ul> 
</li> 
</ol> 
<p>OpenFeint X initialization takes place during your standard OpenFeint initialization<br /> 
  so at this point you can begin utilizing OpenFeint X features!
</p> 
<h4><a name="using"></a>Using Downloadable Content</h4> 
<p>  For paid downloadable content you must use iTunesConnect to configure<br /> 
    your application's In-App-Purchases. A full description of this process is<br /> 
    beyond the scope of this document, but here are some guidelines: </p> 
<ul> 
<li>In-App-Purchase is accessible at <a href="http://itunesconnect.apple.com">http://itunesconnect.apple.com.</a> 
    </li> 
<li> You must have an active paid application's contract to use In-App-Purchase.
    </li> 
<li>Downloadable content In-App-Purchases should always be Non-Consumable products.
    </li> 
<li>Access the DLC Manager section of the developer dashboard at <a href="https://api.openfeint.com/dd/">https://api.openfeint.com/dd/</a>.
    </li> 
<li>Click <strong>OpenFeint X</strong> in the upper right corner.
    </li> 
<li>Click <strong>DLC Manager</strong> on the left navigation menu.
    </li> 
<li>You can configure your DLC here.
    </li> 
<li>For paid DLC, the <strong>Product ID</strong> field must match the Apple In-App-Purchase product ID.
	OpenFeintX will download your In-App-Purchase title, description, and price information using  
	the StoreKit APIs when your application launches so there is no need to duplicate this information in the DLC Manager.
    </li> 
<li>For free DLC, select <strong>No</strong> under <strong>Is this product paid?</strong> and fill in the title and description fields.
    </li> 
<li>Once you have configured your DLC offerings, you can move on to unlocking and<br /> 
    using them in your application.  </li> 
</ul> 
<hr/> 
<h4><a name="changelog"></a>Change Log</h4> 
<hr/> 
<b>Version 2.0.3</b>
<ul>
<li>OFX is now OFSDK-2.12 and GameFeed compatible.</li>
<li>OFX now supports versioning of all store items. For games using OFX versions prior to 2.0.3, the server will assume the game version is 1.0 and filter out any 
assets with a start version higher than that. You can use versioning to add and test new assets that are not visible to live versions of the live game.</li>
<li>You no longer need to use Objective C++ to compile OFX; Objective C is sufficient.</li>
</ul>
<hr/> 
<b>Version 2.0</b>
<ul> 
<li>Support for Downloadable Content by way of non-consumable in-app-purchase
</li> 
<li>Support for free Downloadable Content (no in-app-purchase required)
</li> 
</ul> 
<hr/> 
<p> 
<a href="mailto:documentation@openfent.com?subject=Readme.html OFX 1.0"><i>Comments, Corrections?</i></a> 
</p> 
</div> 

*/
