function disableContextMenu()
{
	try
	{		
	return false;			
	}
	catch(exception)
	{
		return false;
	}
}
document.oncontextmenu=disableContextMenu;
